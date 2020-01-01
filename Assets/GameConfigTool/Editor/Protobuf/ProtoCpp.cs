using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace GCT
{
    internal class ProtoCpp
    {
        public static void Generate(IEnumerable<GCTExcel> excels)
        {
            ProtoHeader.Generate();
            var sw = new Stopwatch();
            sw.Start();
            Parallel.ForEach(excels, PCall.Call<GCTExcel>(GenerateThread));
            sw.Stop();
            Debugger.Log(string.Format("全部Cpp生成完成，耗时{0:N2}秒", (float)sw.ElapsedMilliseconds / 1000));
        }

        private static void GenerateThread(GCTExcel excel)
        {
            var sw = new Stopwatch();
            sw.Start();

            var sb = new StringBuilder();
            var indent = new Indent();
            sb.Append("#ifndef CONFIG_" + excel.name.ToUpper() + "\n");
            sb.Append("#define CONFIG_" + excel.name.ToUpper() + "\n");

            #region include 
            sb.Append("\n");
            sb.Append("#include <map>\n");
            sb.Append("#include <iostream>\n");
            sb.Append("#include <sys/types.h>\n");
            sb.Append("#include <sys/stat.h>\n");
            sb.Append("#include <fcntl.h>\n");
            sb.AppendFormat("#include \"{0}.pb.h\"\n", excel.name);
            #endregion

            #region using
            sb.Append("\n");
            sb.Append("using namespace ::google::protobuf;\n");
            #endregion

            #region class
            sb.Append("\n");
            using (var namespaceScope = new Scope(sb, "namespace Config", new Indent(), "  // namespace Config"))
            {
                sb.Append("\n");
                var classname = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(excel.name) + "Config";
                using (var classScope = new Scope(sb, "class " + classname, new Indent(), "  // class " + classname))
                {
                    indent++;
                    sb.Append(indent.Format("public:\n"));
                    indent++;
                    var arguments = GetArgumentsText(excel, ", ");
                    sb.Append(indent.Format("inline static std::string GetMD5();\n"));
                    sb.Append(indent.Format("inline static std::string GetTimestamp();\n"));
                    sb.Append(indent.Format("inline static bool Has({0});\n", arguments));
                    sb.Append(indent.Format("inline static {1}& Get({0});\n", arguments, excel.name));
                    if (excel.Schema.KeyCount > 1)
                        sb.Append(indent.Format("inline static {1}& Get(const {0}ref& key);\n", excel.name, excel.name));
                    sb.Append(indent.Format("inline static void Load(const std::string& path);\n"));
                    sb.Append(indent.Format("inline static void AddDynamic({0}list& datalist);\n", excel.name));
                    sb.Append(indent.Format("inline static void AddDynamic({0}& data);\n", excel.name));
                    sb.Append(indent.Format("inline static void ClearDynamic();\n"));

                    sb.Append("\n");
                    indent--;
                    sb.Append(indent.Format("private:\n"));
                    indent++;
                    var type = GetTypeText(excel);
                    sb.Append(indent.Format("static {0} DataTable;\n", type));
                    sb.Append(indent.Format("static {0} DynamicDataTable;\n", type));
                    sb.Append(indent.Format("static std::string MD5;\n"));
                    sb.Append("\n");

                    sb.Append(indent.Format("inline static void Add({0}& data, {1}& table);\n", excel.name, type));
                    sb.Append(indent.Format("inline static void Remove({0}, {1}& table);\n", arguments, type));
                }

                #region inline
                sb.Append("\n\n");
                sb.Append("/// inline");
                sb.Append("\n");

                AppendMD5(sb, excel, classname);
                AppendHas(sb, excel, classname);
                AppendGet(sb, excel, classname);
                if (excel.Schema.KeyCount > 1)
                    AppendGetByRef(sb, excel, classname);
                AppendLoad(sb, excel, classname);
                AppendAddDynamic(sb, excel, classname);
                AppendClearDynamic(sb, excel, classname);
                AppendAdd(sb, excel, classname);
                AppendRemove(sb, excel, classname);
                sb.Append("\n\n");

                #endregion
            }
            #endregion

            sb.Append("#endif // CONFIG_" + excel.name.ToUpper() + "\n");

            var path = excel.path.Replace("xlsx", "h").Replace(GCTSettings.Instance.ExcelPath, GCTSettings.Instance.OutputPath + "/cpp");
            FileHelper.WriteAllText(path, sb.ToString());

            sw.Stop();
            Debugger.Log(string.Format("生成{0}.h完成，耗时{1:N2}秒", excel.name, (float)sw.ElapsedMilliseconds / 1000));
        }

        public static void GenerateVersion()
        {
            var sb = new StringBuilder();
            var indent = new Indent();
            sb.Append("#ifndef CONFIG_VERSIONS\n");
            sb.Append("#define CONFIG_VERSIONS\n");

            #region include 
            sb.Append("\n");
            sb.AppendFormat("#include \"versions.pb.h\"\n");
            foreach (var excel in GCTExcelLoader.Excels.Values)
                sb.AppendFormat("#include \"{0}.pb.h\"\n", excel.name);
            #endregion

            #region using
            sb.Append("\n");
            sb.Append("using namespace ::google::protobuf;\n");
            #endregion


                using (var namespaceScope = new Scope(sb, "namespace Config", new Indent(), "  // namespace Config"))
            {
                sb.Append("\n");
                using (var classScope = new Scope(sb, "class Version", new Indent(), "  // class version"))
                {
                    indent++;
                    sb.Append(indent.Format("public:\n"));
                    indent++;
                    sb.Append(indent.Format("inline static differentversions Check(const versions& clientVersions);\n"));
                }
                #region inline
                sb.Append("\n\n");
                sb.Append("/// inline");
                sb.Append("\n");

                indent.Reset();
                sb.Append(indent.Format("inline static differentversions version::Check(const versions& clientVersions) {\n"));
                indent++;
                sb.Append(indent.Format("differentversions result;\n"));
                foreach (var excel in GCTExcelLoader.Excels.Values)
                {
                    sb.Append(indent.Format("if ({0}::GetMD5() != clientVersions.{0}().md5()) {{\n", excel.name));
                    indent++;
                    sb.Append(indent.Format("versiontime* diff = result.add_list();\n"));
                    sb.Append(indent.Format("diff->name = \"{0}\";\n", excel.name));
                    indent--;
                    sb.Append(indent.Format("}\n"));
                }
                sb.Append(indent.Format("return result;\n"));
                indent--;
                sb.Append(indent.Format("}"));
                sb.Append("\n\n");
                #endregion
            }

            sb.Append("#endif // CONFIG_VERSIONS\n");

            FileHelper.WriteAllText(GCTSettings.Instance.OutputPath + "/cpp/version.h", sb.ToString());
        }

        private static string GetArgumentsText(GCTExcel excel, string separator)
        {
            var sb = new StringBuilder();

            foreach (var title in excel.Schema.Titles)
            {
                var field = excel.Schema.Fields[title];
                if (field.IsKey)
                    sb.AppendFormat("{0} {1}{2}", field.Type, field.Name.ToLower(), separator);
            }

            sb.Length -= separator.Length;

            return sb.ToString();
        }

        private static string GetProtoArgumentsText(GCTExcel excel, string separator, string instanceName)
        {
            var sb = new StringBuilder();

            foreach (var title in excel.Schema.Titles)
            {
                var field = excel.Schema.Fields[title];
                if (field.IsKey)
                    sb.AppendFormat("{0}.{1}(){2}", instanceName, field.Name.ToLower(), separator);
            }

            sb.Length -= separator.Length;

            return sb.ToString();
        }

        private static string GetTypeText(GCTExcel excel, int from = 0)
        {
            var sb = new StringBuilder();

            var level = 0;
            var skip = 0;
            foreach (var title in excel.Schema.Titles)
            {
                var field = excel.Schema.Fields[title];
                if (field.IsKey)
                {
                    skip++;
                    if (skip > from)
                    {
                        sb.AppendFormat("std::map<{0}, ", field.Type);
                        level++;
                    }
                }
            }
            sb.Append(excel.name);
            while (level-- > 0)
                sb.Append("> ");

            sb.Length--;
            return sb.ToString();
        }

        private static void AppendMD5(StringBuilder sb, GCTExcel excel, string classname)
        {
            sb.AppendFormat(
@"inline static std::string {0}::GetMD5() {{
    return MD5;
}}", classname);
        }

        private static void AppendHas(StringBuilder sb, GCTExcel excel, string classname)
        {
            var arguments = GetArgumentsText(excel, ", ");
            sb.AppendFormat(
@"
inline static bool {2}::Has({1}) {{
{3}

{4}
    return false;
}}", excel.name, arguments, classname, WalkTable(excel, "DataTable", "return true"), WalkTable(excel, "DynamicDataTable", "return true;"));
        }

        private static void AppendGet(StringBuilder sb, GCTExcel excel, string classname)
        {
            var arguments = GetArgumentsText(excel, ", ");
            sb.AppendFormat(
@"
inline static const {0}& {2}::Get({1}) {{
{3}

{4}
    return {0}::default_instance();
}}", excel.name, arguments, classname, WalkTable(excel, "DataTable", "return {0};"), WalkTable(excel, "DynamicDataTable", "return {0};"));
        }

        private static void AppendGetByRef(StringBuilder sb, GCTExcel excel, string classname)
        {
            var arguments = GetProtoArgumentsText(excel, ", ", "key");
            sb.AppendFormat(
@"
inline static const {0}& {1}::Get(const {0}ref& key) {{
    return Get({2});
}}", excel.name, classname, arguments);
        }

        private static void AppendLoad(StringBuilder sb, GCTExcel excel, string classname)
        {
            sb.AppendFormat(
@"
inline static void {1}::Load(const std::string& path) {{
    int fd = open(path.c_str(), O_RDONLY);
    if (fd < 0) {{
        return;
    }}
    io::FileInputStream* fs = new io::FileInputStream(fd);
    fs.SetCloseOnDelete(true);
    {0}list datalist;
    TextFormat::Parse(fs, &datalist);
    delete fs; fs = NULL;
    
    DataTable.clear();
    for (int i = 0; i < datalist.{0}s_size(); ++i)
        Add(datalist.mutable_{0}s[i], DataTable);

    MD5 = datalist.md5();
}}", excel.name, classname);
        }
        
        private static void AppendAddDynamic(StringBuilder sb, GCTExcel excel, string classname)
        {
            var arguments = GetProtoArgumentsText(excel, ", ", "data");
            sb.AppendFormat(
@"
inline static void {1}::AddDynamic({0}list& datalist) {{
    for (int i = 0; i < datalist.{0}s_size(); ++i)
        AddDynamic(datalist.mutable_{0}s[i]);
}}
inline static void {1}::AddDynamic({0}& data) {{
    Add(data, DynamicDataTable);
}}", excel.name, classname);
        }

        private static void AppendClearDynamic(StringBuilder sb, GCTExcel excel, string classname)
        {
            sb.AppendFormat(
@"
inline static void {0}::ClearDynamic() {{
    DynamicDataTable.clear();
}}", classname);
        }

        private static void AppendAdd(StringBuilder sb, GCTExcel excel, string classname)
        {
            var arguments = GetProtoArgumentsText(excel, "][", "data");
            var type = GetTypeText(excel);
            sb.AppendFormat(
@"
inline static void {3}::Add({0}& data, {1}& table) {{
    table[{2}] = data;
}}", excel.name, type, arguments, classname);
        }

        private static void AppendRemove(StringBuilder sb, GCTExcel excel, string classname)
        {
            var type = GetTypeText(excel);
            var arguments = GetArgumentsText(excel, ", ");
            sb.AppendFormat(
@"
inline static void {2}::Remove({0}, {1}& table) {{
{3}
}}", arguments, type, classname, WalkTable(excel, "table", "{1}.erase({0});"));
        }

        private static string WalkTable(GCTExcel excel, string table, string result)
        {
            var sb = new StringBuilder();
            var indent = new Indent();
            var level = 0;
            string parentTable = string.Empty;
            foreach (var title in excel.Schema.Titles)
            {
                var field = excel.Schema.Fields[title];
                if (field.IsKey)
                {
                    indent++;
                    var type = GetTypeText(excel, level);
                    sb.Append(indent.Format("{0}::iterator it{1} = {2}.find({3});\n", type, field.Name, table, field.Name.ToLower()));
                    sb.Append(indent.Format("if (it{0} != {1}.end()) {{\n", field.Name, table));
                    parentTable = table;
                    table = string.Format("it{0}->second", field.Name);
                    level++;
                }
            }
            indent++;
            if (result.Contains("{1}"))
                sb.Append(indent.Format(result + "\n", table.Substring(0, table.IndexOf('-')), parentTable));
            else if (result.Contains("{0}"))
                sb.Append(indent.Format(result + "\n", table));
            else
                sb.Append(indent.Format(result + "\n"));

            indent--;
            while (indent.Level > 0)
            {
                sb.Append(indent.Format("}\n"));
                indent--;
            }
            sb.Length--;
            return sb.ToString();
        }
    }
}

