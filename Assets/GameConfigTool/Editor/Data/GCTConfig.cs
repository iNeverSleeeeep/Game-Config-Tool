using NPOI.SS.UserModel;
using System.Collections.Generic;

namespace GCT
{
    internal class GCTConfig
    {
        public interface ISlice
        {
            bool Fit(string key);
            bool Fit(long key);
            string ToString(object key);
        }

        public class StringSlice : ISlice
        {
            public string key;

            public bool Fit(string key)
            {
                return this.key == key;
            }
            public bool Fit(long key)
            {
                return false;
            }
            public string ToString(object key)
            {
                return string.Format("{0} == \"{1}\"", key, this.key);
            }
        }

        public class NumberSlice : ISlice
        {
            public long from;
            public long to;

            public bool Fit(string key)
            {
                return false;
            }
            public bool Fit(long key)
            {
                if (from == to)
                    return key == to;
                return key >= from && key < to;
            }
            public string ToString(object key)
            {
                if (from == to)
                    return string.Format("{0} == {1}", key, this.from);
                else
                    return string.Format("{0} >= {1} and {0} < {2}", key, from, to);
            }
        }

        public List<ISlice> Slices = new List<ISlice>();
        public bool IsClientSlice { get { return Slices.Count > 0; } }

        public GCTConfig(ISheet configSheet, GCTExcel excel)
        {
            var rows = configSheet.GetRowEnumerator();
            while (rows.MoveNext())
            {
                var row = rows.Current as IRow;
                if (row.GetCell(0).StringCellValue == "客户端分表")
                    GenerateSlices(row.GetCell(1), excel);
            }
        }

        private void GenerateSlices(ICell cell, GCTExcel excel)
        {
            if (cell == null || cell.CellType != CellType.String)
                return;
            var config = cell.StringCellValue;

            config = config.Remove(0, 1).Replace("]", "");
            var slices = config.Split('[');
            foreach (var slice in slices)
            {
                if (slice.Contains("-"))
                {
                    var numbers = slice.Split('-');
                    Slices.Add(new NumberSlice() { from = long.Parse(numbers[0]), to = long.Parse(numbers[1]) });
                }
                else
                {
                    long num;
                    if (long.TryParse(slice, out num))
                        Slices.Add(new NumberSlice() { from = num, to = num });
                    else
                    {
                        var field = excel.Schema.Fields[excel.Schema.KeyTitles[0]];
                        if (field.Type is GCTTypeKeywords)
                        {
                            var value = GCTKeywords.GetValue(field.Type.ToString(), slice);
                            Slices.Add(new NumberSlice() { from = value, to = value });
                        }
                        else
                            Slices.Add(new StringSlice() { key = slice });
                    }
                }
            }
        }
    }
}