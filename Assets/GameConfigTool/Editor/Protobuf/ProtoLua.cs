using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace GCT
{
    internal class ProtoLua
    {
        public static void Generate(IEnumerable<GCTExcel> excels)
        {
            ProtoHeader.Generate();
            var sw = new Stopwatch();
            sw.Start();
            Parallel.ForEach(excels, PCall.Call<GCTExcel>(GenerateThread));

            sw.Stop();
            Debugger.Log(string.Format("全部Lua生成完成，耗时{0:N2}秒", (float)sw.ElapsedMilliseconds / 1000));
        }

        private static void GenerateThread(GCTExcel excel)
        {
            var sw = new Stopwatch();
            sw.Start();

            GenerateClient(excel);

            GenerateServer(excel);

            sw.Stop();
            Debugger.Log(string.Format("生成{0}.lua完成，耗时{1:N2}秒", excel.name, (float)sw.ElapsedMilliseconds / 1000));
        }

        private static void GenerateClient(GCTExcel excel)
        {
            var table = excel.Data;

            var keys = new StringBuilder();
            var keyIndex = new Dictionary<string, int>();
            
            keys.Append("local s, r = setmetatable, rawget\n");
            keys.Append("local a = {");
            var index = 1;
            foreach (var field in excel.Schema.Fields.Values)
            {
                if (field.IsClient)
                {
                    keyIndex.Add(field.Name, index);
                    keys.AppendFormat(" {0} = {1},", field.Name, index++);
                }
            }
            keys.Length -= 1;
            keys.Append(" }\n");
            keys.Append("local m = { __index = function(t, k) return r(t, a[k]) end, __newindex = function() end }\n");

            if (excel.Config.IsClientSlice)
            {
                var sbList = new List<StringBuilder>();
                var indentList = new List<Indent>();
                foreach (var slice in excel.Config.Slices)
                {
                    var sb = new StringBuilder();
                    var indent = new Indent();
                    sb.Append(keys);
                    sb.Append("return {\n");
                    indent++;
                    sbList.Add(sb);
                    indentList.Add(indent);
                }
                var defaultSb = new StringBuilder();
                var defaultIndent = new Indent();
                defaultSb.Append(keys);
                defaultSb.Append("local d = {\n");
                defaultIndent++;
                
                GenerateTable(table.Data, keyIndex, 0, excel.Schema.KeyCount, defaultSb, defaultIndent, sbList, indentList, excel.Config.Slices, true);

                foreach (var sb in sbList)
                    sb.Append("}\n");
                defaultSb.Append("}\n");

                for (var i = 0; i < sbList.Count; ++i)
                {
                    var path = excel.path.Replace(".xlsx", string.Format("{0}.lua", i));
                    path = path.Replace(GCTSettings.Instance.ExcelPath, GCTSettings.Instance.OutputPath + "/lua");
                    FileHelper.WriteAllText(path, sbList[i].ToString());
                }

                defaultSb.Append("return s(d, { __index = function(t, k)\n");
                for (var i = 0; i < excel.Config.Slices.Count; ++i)
                {
                    var slice = excel.Config.Slices[i];
                    defaultSb.AppendFormat("    if {1} then return require(\"{3}{0}{2}\")[k] end\n", excel.name, slice.ToString("k"), i, GCTSettings.Instance.LuaRequirePath);
                }

                defaultSb.Append("end, __newindex = function() end })\n");
                var defaultPath = excel.path.Replace("xlsx", "lua").Replace(GCTSettings.Instance.ExcelPath, GCTSettings.Instance.OutputPath + "/lua");
                FileHelper.WriteAllText(defaultPath, defaultSb.ToString());
            }
            else
            {
                var sb = new StringBuilder();
                var indent = new Indent();

                sb.Append(keys);
                using (var _ = new Scope(sb, "return", indent))
                    GenerateTable(table.Data, keyIndex, 0, excel.Schema.KeyCount, sb, indent, true);

                var path = excel.path.Replace("xlsx", "lua").Replace(GCTSettings.Instance.ExcelPath, GCTSettings.Instance.OutputPath + "/lua");
                FileHelper.WriteAllText(path, sb.ToString());
            }
        }

        private static void GenerateServer(GCTExcel excel)
        {
            var table = excel.Data;
            var sb = new StringBuilder();
            var indent = new Indent();
            using (var _ = new Scope(sb, "", indent))
            {
                foreach (var rowTable in table)
                {
                    GenerateTable(rowTable as IDictionary<string, object>, sb, false, indent);
                    sb.Append(",\n");
                }
            }

            var path = excel.path.Replace("xlsx", "lua").Replace(GCTSettings.Instance.ExcelPath, GCTSettings.Instance.OutputPath + "/protolua");
            FileHelper.WriteAllText(path, sb.ToString());
        }

        public delegate int GetSliceIndexDelegate(string key);
        private static void GenerateTable(IDictionary<string, object> table, IDictionary<string, int> keyIndex, int deep, int maxDeep, StringBuilder defaultSb, Indent defaultIndent, List<StringBuilder> sbList, List<Indent> indentList, List<GCTConfig.ISlice> slices, bool isClient)
        {
            GetSliceIndexDelegate GetSliceIndex = key =>
            {
                long nkey;
                if (long.TryParse(key, out nkey))
                {
                    for (int i = 0; i < slices.Count; ++i)
                    {
                        if (slices[i].Fit(nkey))
                            return i;

                    }
                    return -1;
                }
                else
                {
                    for (int i = 0; i < slices.Count; ++i)
                    {
                        if (slices[i].Fit(key))
                            return i;
                    }
                    return -1;
                }
            };

            deep++;
            if (deep == maxDeep)
            {
                foreach (var pair in table)
                {
                    StringBuilder sb = defaultSb;
                    Indent indent = defaultIndent;
                    var index = GetSliceIndex(pair.Key);
                    if (index >= 0)
                    {
                        sb = sbList[index];
                        indent = indentList[index];
                    }

                    var key = pair.Key;
                    int value = 0;
                    if (keyIndex == null)
                    {
                        if (int.TryParse(key, out value))
                            sb.Append(indent.Format("[{0}] =", key));
                        else
                            sb.Append(indent.Format("{0} =", key));
                    }
                    else
                    {
                        if (int.TryParse(key, out value))
                            sb.Append(indent.Format("[{0}] = s(", key));
                        else
                            sb.Append(indent.Format("{0} = s(", key));
                    }
                    
                    GenerateTable(pair.Value, sb, isClient);
                    if (keyIndex == null)
                        sb.Append("\n");
                    else
                        sb.Append(" m ),\n");
                }
            }
            else
            {
                foreach (var pair in table)
                {
                    StringBuilder sb = defaultSb;
                    Indent indent = defaultIndent;
                    var index = GetSliceIndex(pair.Key);
                    if (index >= 0)
                    {
                        sb = sbList[index];
                        indent = indentList[index];
                    }
                    long key = 0;
                    string scopeTitle = null;
                    if (long.TryParse(pair.Key, out key))
                        scopeTitle = string.Format("[{0}] =", pair.Key);
                    else
                        scopeTitle = string.Format("{0} =", pair.Key);
                    using (var _ = new Scope(sb, scopeTitle, indent, ","))
                        GenerateTable(pair.Value as IDictionary<string, object>, keyIndex, deep, maxDeep, sb, indent, isClient, 1);

                }
            }
        }

        private static void GenerateTable(IDictionary<string, object> table, IDictionary<string, int> keyIndex, int deep, int maxDeep, StringBuilder sb, Indent indent, bool isClient, int column = 1)
        {
            deep++;

            if (deep == maxDeep)
            {
                var index = 0;
                foreach (var pair in table)
                {
                    var key = pair.Key;
                    if (string.IsNullOrEmpty(key.Trim()))
                        continue;
                    index++;

                    int value = 0;
                    if (keyIndex == null)
                    {
                        if ((index - 1) % column == 0)
                        {
                            if (int.TryParse(key, out value))
                                sb.Append(indent.Format("[{0}] =", key));
                            else
                                sb.Append(indent.Format("{0} =", key));
                        }
                        else
                        {
                            if (int.TryParse(key, out value))
                                sb.Append(string.Format("[{0}] =", key));
                            else
                                sb.Append(string.Format("{0} =", key));
                        }
                    }
                    else
                    {
                        if ((index - 1) % column == 0)
                        {
                            if (int.TryParse(key, out value))
                                sb.Append(indent.Format("[{0}] = s(", key));
                            else
                                sb.Append(indent.Format("{0} = s(", key));
                        }
                        else
                        {
                            if (int.TryParse(key, out value))
                                sb.Append(string.Format("[{0}] = s(", key));
                            else
                                sb.Append(string.Format("{0} = s(", key));
                        }
                    }


                    GenerateTable(pair.Value, sb, isClient, keyIndex);
                    if (keyIndex == null)
                    {
                        if (index % column == 0)
                            sb.Append("\n");
                        else
                            sb.Append(" ");
                    }
                    else
                    {
                        if (index % column == 0)
                            sb.Append(" m ),\n");
                        else
                            sb.Append(" m ), ");
                    }
                }
            }
            else
            {
                foreach (var pair in table)
                {
                    long key = 0;
                    string scopeTitle = null;
                    if (long.TryParse(pair.Key, out key))
                        scopeTitle = string.Format("[{0}] =", pair.Key);
                    else
                        scopeTitle = string.Format("{0} =", pair.Key);
                    using (var _ = new Scope(sb, scopeTitle, indent, ","))
                    {
                        GenerateTable(pair.Value as IDictionary<string, object>, keyIndex, deep, maxDeep, sb, indent, isClient, column);
                    }
                }
            }
        }

        private static void GenerateTable(IDictionary<string, object> dic, StringBuilder sb, bool isClient, Indent indent = null, IDictionary<string, int> keyIndex = null)
        {   
            var content = new StringBuilder();
            if (indent == null)
                content.Append(" {");
            else
                content.Append(indent.ToString() + "{");

            foreach (var pair in dic)
            {
                bool add = true;
                if (dic is GCTRowTable)
                {
                    if (isClient && (dic as GCTRowTable).Client[pair.Key] == false)
                        add = false;
                    if (!isClient && (dic as GCTRowTable).Server[pair.Key] == false)
                        add = false;
                }
                if (add)
                {
                    if (isClient && keyIndex != null)
                        content.AppendFormat(" [{0}] =", keyIndex[pair.Key]);
                    else
                        content.AppendFormat(" {0} =", pair.Key);
                    GenerateTable(pair.Value, content, isClient);
                }
            }
            if (content[content.Length-1] == ',')
                content.Length--;
            content.Append(" }");
            sb.Append(content.ToString());
        }

        private static void GenerateTable(IList<object> list, StringBuilder sb, bool isClient)
        {
            var content = new StringBuilder();
            content.Append(" {");
            foreach (var item in list)
                GenerateTable(item, content, isClient);
            if (content[content.Length - 1] == ',')
                content.Length--;
            content.Append(" }");
            sb.Append(content.ToString());
        }

        private static void GenerateTable(object obj, StringBuilder sb, bool isClient, IDictionary<string, int> keyIndex = null)
        {
            if (obj is string)
                sb.AppendFormat(" \"{0}\",", obj);
            else if (obj is IDictionary<string, object>)
            {
                GenerateTable(obj as IDictionary<string, object>, sb, isClient, null, keyIndex);
                sb.Append(",");
            }
            else if (obj is IList<object>)
            {
                GenerateTable(obj as IList<object>, sb, isClient);
                sb.Append(",");
            }
            else
            {
                sb.AppendFormat(" {0},", obj);
            }
        }

        public static void GenerateKeywords()
        {
            var keywords = new StringBuilder();
            var indent = new Indent();
            using (var _ = new Scope(keywords, "return", indent))
                GenerateTable(GCTKeywords.keywords as IDictionary<string, object>, null, 0, 2, keywords, indent, true, 2);
            FileHelper.WriteAllText(GCTSettings.Instance.OutputPath + "/lua/keywords.lua", keywords.ToString());
        }

        public static void GenerateVersion()
        {
            var version = new StringBuilder();
            var indent = new Indent();
            using (var _ = new Scope(version, "return", indent))
            {
                foreach (var excel in GCTExcelLoader.Excels.Values)
                    version.Append(indent.Format("{0} = {{ md5 = \"{1}\" }},\n", excel.name, excel.md5));
            }
            FileHelper.WriteAllText(GCTSettings.Instance.OutputPath + "/lua/version.lua", version.ToString());
        }
    }
}

