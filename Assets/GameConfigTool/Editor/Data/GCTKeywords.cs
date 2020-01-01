using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GCT
{ 
    internal class Keyword
    {
        public string Name;
        public int Value;
        public string Key;
        public string Describe;

        public override string ToString()
        {
            return Value.ToString();
        }
    }
    internal static class GCTKeywords
    {
        public static IDictionary<string, object> keywords = new Dictionary<string, object>();
        private static Dictionary<string, Dictionary<string, Keyword>> lookup = new Dictionary<string, Dictionary<string, Keyword>>();


        public static int GetValue(string type, string key)
        {
            if (lookup.ContainsKey(type))
                if ((lookup[type]).ContainsKey(key))
                    return (lookup[type])[key].Value;
                else
                    throw new System.Exception("keywords key:" + key + "不存在");
            else
                throw new System.Exception("keywords type:" + type + "不存在");
        }

        public static int GetMaskValue(string type, string mask)
        {
            var keys = mask.Split('|');
            int value = 0;
            foreach (var key in keys)
            {
                value |= GetValue(type, key);
            }
            return value;
        }

        public static string GetDescribe(string type, string key)
        {
            if (lookup.ContainsKey(type))
                if ((lookup[type]).ContainsKey(key))
                    return (lookup[type])[key].Describe;
                else
                    throw new System.Exception("keywords key:" + key + "不存在");
            else
                throw new System.Exception("keywords type:" + type + "不存在");
        }

        public static void Init()
        {
            lookup.Clear();
            keywords.Clear();
            var path = GCTSettings.Instance.IncludePath + "/keywords.proto";
            var lines = File.ReadAllLines(path);
            string type = null;
            foreach (var line in lines)
            {
                if (line.Contains("enum"))
                {
                    type = GetKeywordType(line);
                    lookup.Add(type, new Dictionary<string, Keyword>());
                    keywords.Add(type, new Dictionary<string, object>());
                }
                else if (line.Contains("}"))
                    type = null;
                else if (type != null)
                {
                    var keyword = GetKeyword(line);
                    lookup[type][keyword.Key] = keyword;
                    (keywords[type] as IDictionary<string, object>)[keyword.Name] = keyword.Value;
                    if (string.IsNullOrEmpty(keyword.Describe) == false)
                        (keywords[type] as IDictionary<string, object>)[keyword.Value.ToString()] = keyword.Describe;
                }
            }
        }

        private static string GetKeywordType(string line)
        {
            return line.SubStr("enum".Length, line.IndexOf('{')).Trim();
        }

        private static Keyword GetKeyword(string line)
        {
            var name = line.SubStr(0, line.IndexOf('=')).Trim();
            var value = int.Parse(line.SubStr(line.IndexOf('=') + 1, line.IndexOf(';')).Trim());
            var key = line.Substring(line.IndexOf("//") + 2).Trim();
            if (key.Contains(" "))
                key = key.Substring(0, key.IndexOf(" "));
            var describe = string.Empty;
            if (line.Contains("describe="))
            {
                describe = line.Substring(line.IndexOf("describe=") + "describe=".Length);
                if (describe.Contains(" "))
                    describe = describe.Substring(0, describe.IndexOf(" "));
            }

            return new Keyword()
            {
                Name = name,
                Value = value,
                Key = key,
                Describe = describe,
            };
        }

        public static string SubStr(this string str, int fromIndex, int toIndex)
        {
            return str.Substring(fromIndex, toIndex - fromIndex);
        }
    }
}
