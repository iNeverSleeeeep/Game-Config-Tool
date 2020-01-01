using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

namespace GCT
{
    internal class ProtoGenerator
    {
        public static void GenerateProto(IEnumerable<GCTExcel> excels)
        {
            var sw = new Stopwatch();
            sw.Start();
            ProtoHeader.Generate();
            ProtoHeader.CopyInclude();
            GenerateExcelKey();
            Parallel.ForEach(excels, PCall.Call<GCTExcel>(GenerateProtoThread));

            GenerateVersionProto();
            sw.Stop();
            Debugger.Log(string.Format("全部Proto生成完成，耗时{0:N2}秒", (float)sw.ElapsedMilliseconds / 1000));
        }

        public static void GenerateLua(string path)
        {
            var sw = new Stopwatch();
            sw.Start();
            var files = FileHelper.GetFiles(path, "*.proto");
            foreach (var file in files)
            {
                var newpath = file.Replace("/proto", "/protolua") + ".bytes";
                FileHelper.MakeSureDirectory(newpath);
                var arguments = string.Format(" -o {0} {1}", newpath, FileHelper.GetFileName(file));
                var processInfo = new ProcessStartInfo(GCTSettings.Instance.Protoc, arguments);
                processInfo.WorkingDirectory = FileHelper.GetDirectoryName(file);
                var process = Process.Start(processInfo);
                process.ErrorDataReceived += (sender, e) => Debugger.LogError(e.Data);
                process.OutputDataReceived += (sender, e) => Debugger.Log(e.Data);
                process.WaitForExit();
            }
            sw.Stop();
            Debugger.Log(string.Format("全部ProtoLua生成完成，耗时{0:N2}秒", (float)sw.ElapsedMilliseconds / 1000));
        }

        public static void GenerateCpp(string path)
        {
            var sw = new Stopwatch();
            sw.Start();
            var files = FileHelper.GetFiles(path, "*.proto");
            foreach (var file in files)
            {
                var directory = Path.GetDirectoryName(file);
                var newdirectory = directory.Replace("/proto", "/cpp");
                FileHelper.MakeSureDirectory(file.Replace("/proto", "/cpp"));
                var arguments = string.Format(" -I {2} --cpp_out {0} {1}", newdirectory, file, directory);
                var processInfo = new ProcessStartInfo(GCTSettings.Instance.Protoc, arguments);
                processInfo.WorkingDirectory = FileHelper.GetDirectoryName(file);
                var process = Process.Start(processInfo);
                process.ErrorDataReceived += (sender, e) => Debugger.LogError(e.Data);
                process.OutputDataReceived += (sender, e) => Debugger.Log(e.Data);
                process.WaitForExit();
            }
            sw.Stop();
            Debugger.Log(string.Format("全部ProtoLua生成完成，耗时{0:N2}秒", (float)sw.ElapsedMilliseconds / 1000));
        }

        private static void GenerateProtoThread(GCTExcel excel)
        {
            var sw = new Stopwatch();
            sw.Start();

            var sb = new StringBuilder();
            var indent = new Indent();
            sb.Append(ProtoHeader.Get());
            using (var _ = new Scope(sb, string.Format("message {0}", excel.name), indent))
            {
                for (var i = 0; i < excel.Schema.Titles.Count; ++i)
                {
                    var title = excel.Schema.Titles[i];
                    var field = excel.Schema.Fields[title];
                    if (field.IsServer)
                    {
                        if (field.IsArray)
                            sb.AppendLine(indent.Format("repeated {0} {1} = {2};", field.Type, field.Name, i + 1));
                        else if (field.IsOptional)
                            sb.AppendLine(indent.Format("optional {0} {1} = {2};", field.Type, field.Name, i + 1));
                        else
                            sb.AppendLine(indent.Format("required {0} {1} = {2};", field.Type, field.Name, i + 1));
                    }
                }
            }

            sb.AppendLine();

            using (var _ = new Scope(sb, string.Format("message {0}list", excel.name), indent))
            {
                sb.AppendLine(indent.Format("repeated {0} {0}s = 1;", excel.name));
                sb.AppendLine(indent.Format("required string md5 = 2;"));
            }

            var path = excel.path.Replace("xlsx", "proto").Replace(GCTSettings.Instance.ExcelPath, GCTSettings.Instance.OutputPath + "/proto");
            FileHelper.WriteAllText(path, sb.ToString());

            sw.Stop();
            Debugger.Log(string.Format("生成{0}.proto完成，耗时{1:N2}秒", excel.name, (float)sw.ElapsedMilliseconds / 1000));
        }

        private static List<string> GetRowStringList(IRow row)
        {
            var result = new List<string>();
            short minColIx = row.FirstCellNum;
            short maxColIx = row.LastCellNum;
            for (short colIx = ++minColIx; colIx < maxColIx; colIx++)
            {
                var cell = row.GetCell(colIx);
                if (cell != null)
                    result.Add(cell.StringCellValue);
            }
            return result;
        }

        private static void GenerateVersionProto()
        {
            var sb = new StringBuilder();
            var indent = new Indent();
            sb.AppendLine("syntax = \"proto2\";");
            sb.AppendFormat("package {0};\n\n", ProtoHeader.Package);
            using (var _ = new Scope(sb, string.Format("message versioninfo"), indent))
            {
                sb.Append(indent.Format("required string md5 = 1;\n"));
            }
            sb.Append("\n");
            using (var _ = new Scope(sb, string.Format("message versiontime"), indent))
            {
                sb.Append(indent.Format("required string excel = 1;\n"));
            }
            sb.Append("\n");

            using (var _ = new Scope(sb, string.Format("message versions"), indent))
            {
                var index = 1;
                foreach (var excel in GCTExcelLoader.Excels.Values)
                    sb.Append(indent.Format("required versioninfo {0} = {1};\n", excel.name, index++));
            }
            sb.Append("\n");
            using (var _ = new Scope(sb, string.Format("message differentversions"), indent))
            {
                sb.Append(indent.Format("repeated versiontime list = 1;\n"));
            }
            sb.Append("\n");
            FileHelper.WriteAllText(GCTSettings.Instance.OutputPath + "/proto/versions.proto", sb.ToString());
        }

        private static void GenerateExcelKey()
        {
            var sb = new StringBuilder();
            var indent = new Indent();
            sb.AppendLine("syntax = \"proto2\";");
            sb.AppendLine();
            sb.Append("import \"keywords.proto\";\n");
            sb.AppendLine();
            sb.AppendFormat("package {0};\n", ProtoHeader.Package);
            sb.AppendLine();
            foreach (var excel in GCTExcelLoader.Excels.Values)
            {
                if (excel.Schema.KeyCount > 1)
                {
                    using (var _ = new Scope(sb, string.Format("message {0}ref", excel.name), indent))
                    {
                        for (var i = 0; i < excel.Schema.Keys.Count; ++i)
                        {
                            var keytitle = excel.Schema.KeyTitles[i];
                            var keyfield = excel.Schema.Fields[keytitle];
                            sb.Append(indent.Format("required {0} {1} = {2};\n", keyfield.Type, keyfield.Name, i + 1));
                        }
                    }
                }
            }
            FileHelper.WriteAllText(GCTSettings.Instance.OutputPath + "/proto/excelkeys.proto", sb.ToString());
        }
    }
}
