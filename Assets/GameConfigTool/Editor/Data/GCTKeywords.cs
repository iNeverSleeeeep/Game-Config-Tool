﻿using System.Collections;
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
        public int Index;

        public override string ToString()
        {
            return Value.ToString();
        }

        public static Keyword Empty = new Keyword();
    }
    internal static class GCTKeywords
    {
        public static IDictionary<string, object> keywords = new Dictionary<string, object>();
        private static Dictionary<string, Dictionary<string, Keyword>> lookup = new Dictionary<string, Dictionary<string, Keyword>>();
        private static Dictionary<string, List<string>> fieldsKeys = new Dictionary<string, List<string>>();


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
                if (string.IsNullOrEmpty(key) == false)
                    value |= GetValue(type, key);
            }
            return value;
        }

        public static List<string> GetMaskKeys(string type)
        {
            return fieldsKeys[type];
        }

        public static string GetMaskText(string type, int value)
        {
            string text = "";
            foreach (var keyword in lookup[type] as IDictionary<string, Keyword>)
            {
                if ((keyword.Value.Value & value) != 0 && text.Contains(keyword.Value.Name) == false)
                {
                    if (string.IsNullOrEmpty(text))
                        text = keyword.Key;
                    else
                        text = text + "|" + keyword.Key;
                }
            }
            return text;
        }

        public static int GetIndex(string type, int value)
        {
            return (lookup[type][value.ToString()] as Keyword).Index;
        }

        public static string GetKeyByIndex(string type, int index)
        {
            return fieldsKeys[type][index];
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
            fieldsKeys.Clear();
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
                    fieldsKeys.Add(type, new List<string>());
                    if (type.Contains("Mask") == false)
                    {
                        lookup[type][" "] = Keyword.Empty;
                        lookup[type]["0"] = Keyword.Empty;
                        (keywords[type] as IDictionary<string, object>)[" "] = 0;
                        fieldsKeys[type].Add(" ");
                    }
                }
                else if (line.Contains("}"))
                    type = null;
                else if (type != null)
                {
                    var keyword = GetKeyword(line);
                    lookup[type][keyword.Key] = keyword;
                    lookup[type][keyword.Value.ToString()] = keyword;
                    (keywords[type] as IDictionary<string, object>)[keyword.Name] = keyword.Value;

                    keyword.Index = fieldsKeys[type].Count;
                    fieldsKeys[type].Add(keyword.Key);
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
