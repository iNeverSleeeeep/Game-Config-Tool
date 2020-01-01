using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GCT
{
    internal class GCTData : IEnumerable<IDictionary<string, object>>
    {
        public IDictionary<string, object> Data;
        public GCTExcel Excel { get; private set; }

        public GCTData(GCTExcel excel)
        {
            Excel = excel;
            Data = GetDataTable(excel, excel.Schema.KeyTitles);
        }

        public bool ContainKeys(List<string> keys)
        {
            var current = Data;
            foreach (var key in keys)
            {
                if (current == null)
                    return false;
                if (current.ContainsKey(key) == false)
                    return false;
                current = current[key] as IDictionary<string, object>;
            }
            return true;
        }

        public void Check()
        {
            Check(Data);
        }

        private void Check(IDictionary<string, object> table)
        {
            foreach (var value in table.Values)
            {
                if (value is GCTRowTable)
                    (value as GCTRowTable).Check();
                else
                    Check(value as IDictionary<string, object>);
            }
        }

        private static IDictionary<string, object> GetDataTable(GCTExcel excel, List<string> keys)
        {
            IDictionary<string, object> table = new SortedDictionary<string, object>();
            GetRowTable(excel.DataSheet.GetRowEnumerator(), keys, table, excel);
            foreach (var sheet in excel.SubDataSheet)
                GetRowTable(sheet.GetRowEnumerator(), keys, table, excel);

            return table;
        }

        private static void GetRowTable(IEnumerator rows, List<string> keys, IDictionary<string, object> table, GCTExcel excel)
        {
            IRow titleRow = null;
            while (rows.MoveNext())
            {
                var current = table;
                var row = rows.Current as IRow;
                if (titleRow == null)
                    titleRow = row;
                else
                {
                    var currentKeys = new List<string>();
                    short minColIx = row.FirstCellNum;
                    short maxColIx = row.LastCellNum;
                    short keyIndex = 0;
                    for (short colIx = minColIx; colIx < maxColIx; colIx++)
                    {
                        var cell = row.GetCell(colIx);
                        var titleCell = titleRow.GetCell(colIx);
                        if (titleCell.StringCellValue == keys[keyIndex])
                        {
                            if (cell == null || string.IsNullOrEmpty(cell.Value()))
                            {
                                Debugger.LogError(string.Format("键为空 Excel:{0} Key:{1}", excel.name, titleCell.StringCellValue));
                                break;
                            }
                            if (keyIndex == keys.Count - 1 && current.ContainsKey(cell.Value()))
                            {
                                Debugger.LogError(string.Format("键重复 Excel:{0} Key:{1}", excel.name, string.Concat(currentKeys.ToArray())));
                                break;
                            }

                            var field = excel.Schema.Fields[titleCell.StringCellValue];
                            var value = field.Value(new List<ICell>() { cell }, new List<string>() { titleCell.StringCellValue }).ToString();
                            if (current.ContainsKey(value) == false)
                            {
                                current.Add(value, new SortedDictionary<string, object>());
                                currentKeys.Add(value);
                            }

                            keyIndex++;
                            if (keyIndex == keys.Count)
                            {
                                current[value] = new GCTRowTable(row, titleRow, excel);
                                break;
                            }
                            else
                            {
                                current = current[value] as IDictionary<string, object>;
                            }
                        }
                    }
                }
            }
        }

        IEnumerator<IDictionary<string, object>> IEnumerable<IDictionary<string, object>>.GetEnumerator()
        {
            return new GCTConfigTableEnumerator(this);
        }

        public IEnumerator GetEnumerator()
        {
            return new GCTConfigTableEnumerator(this);
        }
    }

    internal class GCTConfigTableEnumerator : IEnumerator<IDictionary<string, object>>
    {
        private List<IDictionary<string, object>> m_Current;
        private IEnumerator<IDictionary<string, object>> m_CurrentEnumerator;

        public IDictionary<string, object> Current { get { return m_CurrentEnumerator.Current; } }

        object IEnumerator.Current { get { return m_CurrentEnumerator.Current; } }

        public GCTConfigTableEnumerator(GCTData table)
        {
            m_Current = new List<IDictionary<string, object>>();
            GenerateList(m_Current, table.Data);
            m_CurrentEnumerator = m_Current.GetEnumerator();
        }

        private void GenerateList(List<IDictionary<string, object>> list, IDictionary<string, object> table)
        {
            foreach (var pair in table)
            {
                if (pair.Value is GCTRowTable)
                    list.Add(pair.Value as GCTRowTable);
                else if (pair.Value is IDictionary<string, object>)
                    GenerateList(list, pair.Value as IDictionary<string, object>);
                else
                {
                    Debugger.LogError("逻辑错误");
                    return;
                }
            }
        }

        public void Dispose()
        {
            m_CurrentEnumerator.Dispose();
        }

        public bool MoveNext()
        {
            return m_CurrentEnumerator.MoveNext();
        }

        public void Reset()
        {
            m_CurrentEnumerator.Reset();
        }
    }
}
