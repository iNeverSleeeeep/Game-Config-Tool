using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GCT
{ 
    internal class CommonType
    {
        public List<GCTField> Fields = new List<GCTField>();
    }
    internal static class GCTCommonTypes
    {
        private static Dictionary<string, CommonType> types = new Dictionary<string, CommonType>();
        public static void Init()
        {
            types.Clear();

            var path = GCTSettings.Instance.IncludePath + "/commontypes.proto";
            var lines = File.ReadAllLines(path);
            string typename = null;
            foreach (var line in lines)
            {
                if (line.Contains("message"))
                {
                    typename = GetCommonTypeName(line);
                    if (typename.EndsWith("ref"))
                        Debugger.LogError("通用类型不能以ref结尾 " + typename);
                    else
                        types.Add(typename, new CommonType());
                }
                else if (line.Contains("}"))
                {
                    typename = null;
                }
                else if (typename != null)
                {
                    string fieldtype, fieldname;
                    bool isArray;
                    GetField(line, out fieldtype, out fieldname, out isArray);
                    var title = GetTitle(line);
                    var fulltypename = GCTTypeCreator.FullTypeName(fieldtype);
                    var field = new GCTField(title);
                    field.Name = fieldname;
                    field.Type = GCTTypeCreator.New(fulltypename);
                    field.ColumnCount = 0; // FIXME
                    types[typename].Fields.Add(field);
                }
            }
        }

        public static CommonType Get(string name)
        {
            if (types.ContainsKey(name))
                return types[name];
            else
                throw new System.Exception("CommonType不存在:" + name);
        }

        private static string GetCommonTypeName(string line)
        {
            return line.SubStr("message".Length, line.IndexOf('{')).Trim();
        }

        private static void GetField(string line, out string fieldtype, out string fieldname, out bool isArray)
        {
            isArray = line.Contains("repeated");
            var pretext = isArray ? "repeated" : "required";
            var typeAndName = line.SubStr(line.IndexOf(pretext) + pretext.Length, line.IndexOf('=')).Trim().Split(' ');
            var type = typeAndName[0];
            var name = typeAndName[1];
            if (line.Contains("type="))
            {
                type = line.Substring(line.IndexOf("type=") + "type=".Length);
                if (type.Contains(" "))
                    type = type.Substring(0, type.IndexOf(" "));
            }
            fieldtype = type + (isArray ? "[]" : string.Empty);
            fieldname = name;
        }
        private static string GetTitle(string line)
        {
            var title = line.Substring(line.IndexOf("//") + 2).Trim();
            if (title.Contains(" "))
                title = title.Substring(0, title.IndexOf(" "));
            return title;
        }
    }
}
