using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GCT
{
    internal static class GCTTypeCreator
    {
        private static Dictionary<string, Type> SupportedTypes = new Dictionary<string, Type>
        {
            { "uint32", typeof(GCTTypeUInt32) },
            { "int32", typeof(GCTTypeInt32) },
            { "uint64", typeof(GCTTypeUInt64) },
            { "int64", typeof(GCTTypeInt64) },
            { "double", typeof(GCTTypeDouble) },
            { "float", typeof(GCTTypeFloat) },
            { "bool", typeof(GCTTypeBool) },
            { "string", typeof(GCTTypeString) },
            { "keywords", typeof(GCTTypeKeywords) },
            { "commontypes", typeof(GCTTypeCommonTypes) },
            { "excels", typeof(GCTTypeExcels) },
            { "json", typeof(GCTTypeJson) },
        };

        public static GCTType New(string name)
        {
            var typename = name;
            if (name.Contains("."))
            {
                typename = name.Substring(0, name.IndexOf("."));
                name = name.Substring(typename.Length + 1);
            }
            if (name.EndsWith("[]"))
                return new GCTTypeArray(name.Substring(0, name.Length - 2));
            if (SupportedTypes.ContainsKey(typename))
                return Activator.CreateInstance(SupportedTypes[typename], name) as GCTType;
            else
            {
                Debugger.LogError("不支持的类型 " + typename + " " + name);
                return null;
            }
        }

        public static string FullTypeName(string name)
        {
            if (SupportedTypes.ContainsKey(name))
                return name;
            else if (GCTKeywords.keywords.ContainsKey(name))
                return "keywords." + name;
            else if (name.EndsWith("ref"))
                return "excels." + name.Substring(0, name.Length - 3);
            else
                return "commontypes." + name;
        }
    }

    internal class GCTType
    {
        protected string m_Type;
        public GCTType(string type)
        {
            m_Type = type;
        }

        public override string ToString() { return "error"; }

        public virtual object ToValue(List<ICell> cells, List<string> titles) { return null; }

        public virtual bool Check(List<ICell> cells, List<string> titles) { return true; }

        protected static int GetArrayLength(List<string> titles, int startIndex, string title)
        {
            for (var i = startIndex; i < titles.Count; ++i)
            {
                if (titles[i].StartsWith(title) == false)
                    return i - startIndex;
            }
            return titles.Count - startIndex;
        }

        public virtual void Draw(List<ICell> cells, List<string> titles)
        {
            var value = ToValue(cells, titles);
            EditorGUILayout.LabelField(value.ToString());
        }
    }

    internal class GCTTypeUInt32 : GCTType
    {
        public GCTTypeUInt32(string type) : base(type)
        {
        }

        public override string ToString() { return "uint32"; }

        public override object ToValue(List<ICell> cells, List<string> titles)
        {
            if (cells[0] == null)
                return null;
            return cells[0].NumericCellValue;
        }

        public override void Draw(List<ICell> cells, List<string> titles)
        {
            string value = string.Empty;
            if (cells[0] != null)
                value = ToValue(cells, titles).ToString();
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.TextField(value);
            if (EditorGUI.EndChangeCheck())
            {
                uint uint32Value = 0;
                if (uint.TryParse(value, out uint32Value))
                    cells[0].SetCellValue(uint32Value);
            }
        }
    }

    internal class GCTTypeInt32 : GCTType
    {
        public GCTTypeInt32(string type) : base(type)
        {
        }

        public override string ToString() { return "int32"; }

        public override object ToValue(List<ICell> cells, List<string> titles)
        {
            if (cells[0] == null)
                return null;
            return cells[0].NumericCellValue;
        }

        public override void Draw(List<ICell> cells, List<string> titles)
        {
            string value = string.Empty;
            if (cells[0] != null)
                value = ToValue(cells, titles).ToString();
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.TextField(value);
            if (EditorGUI.EndChangeCheck())
            {
                int int32Value = 0;
                if (int.TryParse(value, out int32Value))
                    cells[0].SetCellValue(int32Value);
            }
        }
    }

    internal class GCTTypeUInt64 : GCTType
    {
        public GCTTypeUInt64(string type) : base(type)
        {
        }

        public override string ToString() { return "uint64"; }

        public override object ToValue(List<ICell> cells, List<string> titles)
        {
            if (cells[0] == null)
                return null;
            return cells[0].NumericCellValue;
        }

        public override void Draw(List<ICell> cells, List<string> titles)
        {
            string value = string.Empty;
            if (cells[0] != null)
                value = ToValue(cells, titles).ToString();
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.TextField(value);
            if (EditorGUI.EndChangeCheck())
            {
                ulong uint64Value = 0;
                if (ulong.TryParse(value, out uint64Value))
                    cells[0].SetCellValue(uint64Value);
            }
        }
    }

    internal class GCTTypeInt64 : GCTType
    {
        public GCTTypeInt64(string type) : base(type)
        {
        }

        public override string ToString() { return "int64"; }

        public override object ToValue(List<ICell> cells, List<string> titles)
        {
            if (cells[0] == null)
                return null;
            return cells[0].NumericCellValue;
        }

        public override void Draw(List<ICell> cells, List<string> titles)
        {
            string value = string.Empty;
            if (cells[0] != null)
                value = ToValue(cells, titles).ToString();
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.TextField(value);
            if (EditorGUI.EndChangeCheck())
            {
                long int64Value = 0;
                if (long.TryParse(value, out int64Value))
                    cells[0].SetCellValue(int64Value);
            }
        }
    }

    internal class GCTTypeDouble : GCTType
    {
        public GCTTypeDouble(string type) : base(type)
        {
        }

        public override string ToString() { return "double"; }

        public override object ToValue(List<ICell> cells, List<string> titles)
        {
            if (cells[0] == null)
                return null;
            return cells[0].NumericCellValue;
        }

        public override void Draw(List<ICell> cells, List<string> titles)
        {
            string value = string.Empty;
            if (cells[0] != null)
                value = ToValue(cells, titles).ToString();
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.TextField(value);
            if (EditorGUI.EndChangeCheck())
            {
                double doubleValue = 0;
                if (double.TryParse(value, out doubleValue))
                    cells[0].SetCellValue(doubleValue);
            }
        }
    }

    internal class GCTTypeFloat : GCTType
    {
        public GCTTypeFloat(string type) : base(type)
        {
        }

        public override string ToString() { return "float"; }

        public override object ToValue(List<ICell> cells, List<string> titles)
        {
            if (cells[0] == null)
                return null;
            return cells[0].NumericCellValue;
        }

        public override void Draw(List<ICell> cells, List<string> titles)
        {
            string value = string.Empty;
            if (cells[0] != null)
                value = ToValue(cells, titles).ToString();
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.TextField(value);
            if (EditorGUI.EndChangeCheck())
            {
                float floatValue = 0;
                if (float.TryParse(value, out floatValue))
                    cells[0].SetCellValue(floatValue);
            }
        }
    }

    internal class GCTTypeBool : GCTType
    {
        public GCTTypeBool(string type) : base(type)
        {
        }

        public override string ToString() { return "bool"; }

        public override object ToValue(List<ICell> cells, List<string> titles)
        {
            if (cells[0] == null)
                return null;
            return cells[0].StringCellValue == "是";
        }

        private static string[] Display = new string[] { "否", "是" };

        public override void Draw(List<ICell> cells, List<string> titles)
        {
            if (cells[0] == null)
                return;
            var value = (bool)ToValue(cells, titles);
            EditorGUI.BeginChangeCheck();
            var result = EditorGUILayout.Popup(value ? 1 : 0, Display);
            if (EditorGUI.EndChangeCheck())
                cells[0].SetCellValue(Display[result]);
        }
    }

    internal class GCTTypeString : GCTType
    {
        public GCTTypeString(string type) : base(type)
        {
        }

        public override string ToString() { return "string"; }

        public override object ToValue(List<ICell> cells, List<string> titles)
        {
            if (cells[0] == null)
                return null;
            return cells[0].Value();
        }

        public override void Draw(List<ICell> cells, List<string> titles)
        {
            string value = string.Empty;
            if (cells[0] != null)
                value = ToValue(cells, titles).ToString();
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.TextField(value);
            if (EditorGUI.EndChangeCheck())
                cells[0].SetCellValue(value);
        }
    }

    internal class GCTTypeKeywords : GCTType
    {
        public GCTTypeKeywords(string type) : base(type)
        {

        }

        public override string ToString() { return m_Type; }

        public override object ToValue(List<ICell> cells, List<string> titles)
        {
            if (cells[0] == null)
                return null;
            return GCTKeywords.GetMaskValue(ToString(), cells[0].Value());
        }

        public override void Draw(List<ICell> cells, List<string> titles)
        {
            int value = 0;
            if (cells[0] != null)
                value = (int)ToValue(cells, titles);
            if (ToString().EndsWith("Mask"))
            {
                var showMixedValue = EditorGUI.showMixedValue;
                EditorGUI.showMixedValue = false;
                EditorGUI.BeginChangeCheck();
                value = EditorGUILayout.MaskField(value, GCTKeywords.GetMaskKeys(ToString()).ToArray());
                if (EditorGUI.EndChangeCheck())
                    cells[0].SetCellValue(GCTKeywords.GetMaskText(ToString(), value));
                EditorGUI.showMixedValue = showMixedValue;
            }
            else
            {
                var index = GCTKeywords.GetIndex(ToString(), value);
                EditorGUI.BeginChangeCheck();
                index = EditorGUILayout.Popup(index, GCTKeywords.GetMaskKeys(ToString()).ToArray());
                if (EditorGUI.EndChangeCheck())
                    cells[0].SetCellValue(GCTKeywords.GetKeyByIndex(ToString(), index));
            }
        }
    }

    internal class GCTTypeCommonTypes : GCTType
    {
        private CommonType m_CommonType;

        public GCTTypeCommonTypes(string type) : base(type)
        {
            m_CommonType = GCTCommonTypes.Get(type);
        }

        public override string ToString() { return m_Type; }

        public override object ToValue(List<ICell> cells, List<string> titles)
        {
            if (cells[0] == null)
                return null;
            IDictionary<string, object> result = new Dictionary<string, object>();
            int cellStartIndex = 0;
            var title = titles[0].Substring(0, titles[0].IndexOf('-'));
            var subtitles = new List<string>();
            foreach (var fulltitle in titles)
                subtitles.Add(fulltitle.Substring(title.Length + 1));
            for (var fieldIndex = 0; fieldIndex < m_CommonType.Fields.Count; ++fieldIndex)
            {
                var field = m_CommonType.Fields[fieldIndex];
                var columnCount = 0;
                if (field.IsArray)
                    columnCount = GetArrayLength(subtitles, cellStartIndex, field.Title);
                else
                {
                    for (var i = cellStartIndex; i < subtitles.Count; ++i)
                    {
                        if (subtitles[i] == field.Title)
                        {
                            columnCount = 1;
                            break;
                        }
                        else if (subtitles[i].StartsWith(field.Title + "-"))
                            columnCount++;
                        else
                            break;
                    }
                }

                result.Add(field.Name, field.Value(cells.GetRange(cellStartIndex, columnCount), subtitles.GetRange(cellStartIndex, columnCount)));
                cellStartIndex += columnCount;
            }
            return result;
        }

        public override bool Check(List<ICell> cells, List<string> titles)
        {
            if (cells[0] == null)
                return true;
            IDictionary<string, object> result = new Dictionary<string, object>();
            int cellStartIndex = 0;
            var title = titles[0].Substring(0, titles[0].IndexOf('-'));
            var subtitles = new List<string>();
            foreach (var fulltitle in titles)
                subtitles.Add(fulltitle.Substring(title.Length + 1));
            for (var fieldIndex = 0; fieldIndex < m_CommonType.Fields.Count; ++fieldIndex)
            {
                var field = m_CommonType.Fields[fieldIndex];
                var columnCount = 0;
                if (field.IsArray)
                    columnCount = GetArrayLength(subtitles, cellStartIndex, field.Title);
                else
                {
                    for (var i = cellStartIndex; i < subtitles.Count; ++i)
                    {
                        if (subtitles[i] == field.Title)
                        {
                            columnCount = 1;
                            break;
                        }
                        else if (subtitles[i].StartsWith(field.Title + "-"))
                            columnCount++;
                        else
                            break;
                    }
                }

                if (field.Type.Check(cells.GetRange(cellStartIndex, columnCount), subtitles.GetRange(cellStartIndex, columnCount)) == false)
                    return false;
                cellStartIndex += columnCount;
            }
            return true;
        }
    }

    internal class GCTTypeArray : GCTType
    {
        private GCTField m_Field;
        public GCTTypeArray(string type) : base(type)
        {
            m_Field = new GCTField(string.Empty);
            m_Field.Type = GCTTypeCreator.New(GCTTypeCreator.FullTypeName(type));
            m_Field.IsOptional = true;
        }

        public override string ToString() { return m_Type; }

        public override object ToValue(List<ICell> cells, List<string> titles)
        {
            if (cells[0] == null)
                return new List<object>();
            var title = titles[0].Substring(0, titles[0].IndexOf("[0]"));
            List<object> values = new List<object>();
            int arrayIndex = 0;
            for (var startIndex = 0; startIndex < cells.Count;)
            {
                var titleIndex = title + string.Format("[{0}]", arrayIndex);
                var length = GetArrayLength(titles, startIndex, titleIndex);
                var subtitles = titles.GetRange(startIndex, length);

                var value = m_Field.Value(cells.GetRange(startIndex, length), subtitles);
                startIndex += length;
                arrayIndex++;
                if (value != null)
                    values.Add(value);
                else
                    break;
            }
            return values;
        }

        public override bool Check(List<ICell> cells, List<string> titles)
        {
            if (cells[0] == null)
                return true;
            var title = titles[0].Substring(0, titles[0].IndexOf("[0]"));
            List<object> values = new List<object>();
            int arrayIndex = 0;
            for (var startIndex = 0; startIndex < cells.Count;)
            {
                var titleIndex = title + string.Format("[{0}]", arrayIndex);
                var length = GetArrayLength(titles, startIndex, titleIndex);
                var subtitles = titles.GetRange(startIndex, length);

                if (m_Field.Type.Check(cells.GetRange(startIndex, length), subtitles) == false)
                    return false;
                startIndex += length;
                arrayIndex++;
            }
            return true;
        }

        public override void Draw(List<ICell> cells, List<string> titles)
        {
            if (cells[0] == null)
                return;
            var title = titles[0].Substring(0, titles[0].IndexOf("[0]"));
            int arrayIndex = 0;
            for (var startIndex = 0; startIndex < cells.Count;)
            {
                var titleIndex = title + string.Format("[{0}]", arrayIndex);
                var length = GetArrayLength(titles, startIndex, titleIndex);
                var subtitles = titles.GetRange(startIndex, length);

                EditorGUILayout.BeginHorizontal();
                m_Field.Type.Draw(cells.GetRange(startIndex, length), subtitles);
                EditorGUILayout.EndHorizontal();
                startIndex += length;
                arrayIndex++;
            }
        }
    }

    internal class GCTTypeExcels : GCTType
    {
        public GCTTypeExcels(string type) : base(type)
        {

        }

        public override string ToString()
        {
            var excel = GCTExcelLoader.GetExcel(m_Type);
            if (excel.Schema.KeyCount > 1)
                return m_Type + "ref";
            else
            {
                var field = excel.Schema.Fields[excel.Schema.KeyTitles[0]];
                return field.Type.ToString();
            }
        }

        public override object ToValue(List<ICell> cells, List<string> titles)
        {
            if (cells[0] == null)
                return null;
            var excel = GCTExcelLoader.GetExcel(m_Type);
            if (excel.Schema.KeyCount > 1)
            {
                var keys = new List<string>();
                if (cells.Count == 1)
                {
                    var values = cells[0].Value().Split('-');
                    for (var i = 0; i < values.Length; ++i)
                    {
                        var field = excel.Schema.Fields[excel.Schema.KeyTitles[i]];
                        if (field.Type is GCTTypeKeywords)
                            keys.Add(GCTKeywords.GetValue(field.Type.ToString(), values[i]).ToString());
                        else
                            keys.Add(values[i]);
                    }
                }
                else
                {
                    for (var i = 0; i < cells.Count; ++i)
                    {
                        var value = cells[i].Value();
                        var field = excel.Schema.Fields[excel.Schema.KeyTitles[i]];
                        if (field.Type is GCTTypeKeywords)
                            keys.Add(GCTKeywords.GetValue(field.Type.ToString(), value).ToString());
                        else
                            keys.Add(value);
                    }
                }

                IDictionary<string, object> result = new Dictionary<string, object>();

                for (var i = 0; i < keys.Count; ++i)
                {
                    var title = excel.Schema.Titles[i];
                    var field = excel.Schema.Fields[title];
                    long value;
                    if (long.TryParse(keys[i], out value))
                        result.Add(excel.Schema.Keys[i], value);
                    else
                        result.Add(excel.Schema.Keys[i], keys[i]);
                }
                return result;
            }
            else
            {
                var field = excel.Schema.Fields[excel.Schema.KeyTitles[0]];
                if (field.Type is GCTTypeKeywords)
                    return GCTKeywords.GetValue(field.Type.ToString(), cells[0].Value());
                else
                    return cells[0].RawValue();
            }
        }

        public override bool Check(List<ICell> cells, List<string> titles)
        {
            if (cells[0] == null)
                return true;

            var excel = GCTExcelLoader.GetExcel(m_Type);
            if (excel.Schema.KeyCount > 1)
            {
                var keys = new List<string>();
                if (cells.Count == 1)
                {
                    var values = cells[0].Value().Split('-');
                    for (var i = 0; i < values.Length; ++i)
                    {
                        var field = excel.Schema.Fields[excel.Schema.KeyTitles[i]];
                        if (field.Type is GCTTypeKeywords)
                            keys.Add(GCTKeywords.GetValue(field.Type.ToString(), values[i]).ToString());
                        else
                            keys.Add(values[i]);
                    }
                }
                else
                {
                    for (var i = 0; i < cells.Count; ++i)
                    {
                        var value = cells[i].Value();
                        var field = excel.Schema.Fields[excel.Schema.KeyTitles[i]];
                        if (field.Type is GCTTypeKeywords)
                            keys.Add(GCTKeywords.GetValue(field.Type.ToString(), value).ToString());
                        else
                            keys.Add(value);
                    }
                }
                return excel.Data.ContainKeys(keys);
            }
            else
            {
                var field = excel.Schema.Fields[excel.Schema.KeyTitles[0]];
                if (field.Type is GCTTypeKeywords)
                {
                    var value = GCTKeywords.GetValue(field.Type.ToString(), cells[0].Value()).ToString();
                    return excel.Data.ContainKeys(new List<string>() { value });
                }
                else
                    return excel.Data.ContainKeys(new List<string>() { cells[0].Value() });
            }
        }
    }
    internal class GCTTypeJson : GCTType
    {
        public GCTTypeJson(string type) : base(type)
        {

        }
        public override string ToString()
        {
            return "json";
        }

        public override object ToValue(List<ICell> cells, List<string> titles)
        {
            if (cells[0] == null)
                return null;
            return SharpJson.JsonDecoder.DecodeText(cells[0].StringCellValue);
        }

        public override void Draw(List<ICell> cells, List<string> titles)
        {
            var value = ToValue(cells, titles) as string;
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.TextField(value);
            if (EditorGUI.EndChangeCheck())
                cells[0].SetCellValue(value);
        }
    }
}
