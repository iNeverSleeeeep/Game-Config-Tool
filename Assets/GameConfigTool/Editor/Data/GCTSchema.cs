using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;

namespace GCT
{
    internal class GCTSchema
    {
        public static readonly string ExportType = "类型";
        public static readonly string ExportName = "导出名";
        public static readonly string AsKey = "Key";
        public static readonly string Client = "客户端";
        public static readonly string Server = "服务器";
        public static readonly string Optional = "选填";

        public static readonly string Mark = "是";
        
        public IDictionary<string, GCTField> Fields;
        public IList<string> Titles = new List<string>();
        public List<string> KeyTitles { get; private set; }
        public List<string> Keys { get; private set; }
        public int KeyCount { get { return Keys.Count; } }

        #region FieldSorter
        internal class FieldSorter : IComparer<string>
        {
            IList<string> titles;
            public FieldSorter(IList<string> titles)
            {
                this.titles = titles;
            }
            public int Compare(string x, string y)
            {
                int xIndex = titles.IndexOf(x);
                int yIndex = titles.IndexOf(y);
                return xIndex.CompareTo(yIndex);
            }
        }
        #endregion

        public GCTSchema(ISheet sheet, IRow dataTitleRow)
        {

            var rows = sheet.GetRowEnumerator();
            rows.MoveNext();
            var titleRow = rows.Current as IRow;
            Titles = GetTitles(titleRow);
            Fields = new SortedDictionary<string, GCTField>(new FieldSorter(Titles));
            foreach (var title in Titles)
            {
                var columnCount = GetFieldColumnCount(dataTitleRow.Cells, title);
                var field = new GCTField(title);
                Fields.Add(title, field);
                field.ColumnCount = columnCount;
            }

            while (rows.MoveNext())
            {
                var row = rows.Current as IRow;
                var cell0 = row.GetCell(0);
                if (cell0 == null)
                    Debugger.LogError("cell0 == null");
                else
                {
                    if (cell0.StringCellValue == ExportType)
                        GenerateType(Fields, titleRow, row);
                    else if (cell0.StringCellValue == ExportName)
                        GenerateName(Fields, titleRow, row);
                    else if (cell0.StringCellValue == AsKey)
                        GenerateIsKey(Fields, titleRow, row);
                    else if (cell0.StringCellValue == Client)
                        GenerateClient(Fields, titleRow, row);
                    else if (cell0.StringCellValue == Server)
                        GenerateServer(Fields, titleRow, row);
                    else if (cell0.StringCellValue == Optional)
                        GenerateOptional(Fields, titleRow, row);
                    else
                    {
                        Debugger.LogWarning("Schema没有定义:" + cell0.StringCellValue);
                    }
                }
            }

            KeyTitles = new List<string>();
            Keys = new List<string>();
            GenerateKeys(this, KeyTitles, Keys);
        }

        public static int GetFieldColumnCount(List<ICell> dataTitleCells, string title)
        {
            var columnCount = 0;
            var isArray = false;
            var isCommonType = false;
            for (var column = 0; column < dataTitleCells.Count; ++column)
            {
                var cell = dataTitleCells[column];
                if (cell.StringCellValue == title)
                {
                    columnCount = 1;
                    break;
                }
                else if (cell.StringCellValue.StartsWith(title + "["))
                {
                    isArray = true;
                    columnCount++;
                }
                else if (cell.StringCellValue.StartsWith(title + "-"))
                {
                    isCommonType = true;
                    columnCount++;
                }
                else if (isArray || isCommonType)
                    break;
            }
            return columnCount;
        }

        private static List<string> GetTitles(IRow titleRow)
        {
            var titles = new List<string>();
            for (var column = 1; column < titleRow.LastCellNum; ++column)
            {
                var cell = titleRow.GetCell(column);
                if (cell == null)
                    Debugger.LogError("Schema错误 " + titleRow.Sheet.Workbook);
                else
                {
                    titles.Add(cell.StringCellValue);
                }
            }
            return titles;
        }

        private static void GenerateType(IDictionary<string, GCTField> fields, IRow titleRow, IRow typeRow)
        {
            for (var column = 1; column < titleRow.LastCellNum; ++column)
            {
                var titleCell = titleRow.GetCell(column);
                var cell = typeRow.GetCell(column);
                var type = GCTTypeCreator.New(cell.StringCellValue);
                fields[titleCell.StringCellValue].Type = type;
            }
        }

        private static void GenerateName(IDictionary<string, GCTField> fields, IRow titleRow, IRow nameRow)
        {
            for (var column = 1; column < titleRow.LastCellNum; ++column)
            {
                var titleCell = titleRow.GetCell(column);
                var cell = nameRow.GetCell(column);
                fields[titleCell.StringCellValue].Name = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(cell.StringCellValue);
            }
        }

        private static void GenerateIsKey(IDictionary<string, GCTField> fields, IRow titleRow, IRow isKeyRow)
        {
            for (var column = 1; column < titleRow.LastCellNum; ++column)
            {
                var titleCell = titleRow.GetCell(column);
                var cell = isKeyRow.GetCell(column);
                fields[titleCell.StringCellValue].IsKey = GetCellBooleanValue(cell);
            }
        }

        private static void GenerateClient(IDictionary<string, GCTField> fields, IRow titleRow, IRow isClientRow)
        {
            for (var column = 1; column < titleRow.LastCellNum; ++column)
            {
                var titleCell = titleRow.GetCell(column);
                var cell = isClientRow.GetCell(column);
                fields[titleCell.StringCellValue].IsClient = GetCellBooleanValue(cell);
            }
        }

        private static void GenerateServer(IDictionary<string, GCTField> fields, IRow titleRow, IRow isServerRow)
        {
            for (var column = 1; column < titleRow.LastCellNum; ++column)
            {
                var titleCell = titleRow.GetCell(column);
                var cell = isServerRow.GetCell(column);
                fields[titleCell.StringCellValue].IsServer = GetCellBooleanValue(cell);
            }
        }

        private static void GenerateOptional(IDictionary<string, GCTField> fields, IRow titleRow, IRow isOptionalRow)
        {
            for (var column = 1; column < titleRow.LastCellNum; ++column)
            {
                var titleCell = titleRow.GetCell(column);
                var cell = isOptionalRow.GetCell(column);
                fields[titleCell.StringCellValue].IsOptional = GetCellBooleanValue(cell);
            }
        }

        private static bool GetCellBooleanValue(ICell cell)
        {
            if (cell == null)
                return false;
            return cell.StringCellValue == Mark;
        }

        private static void GenerateKeys(GCTSchema schema, List<string> keyNames, List<string> keys)
        {
            foreach (var title in schema.Titles)
            {
                var field = schema.Fields[title];
                if (field.IsKey)
                {
                    keyNames.Add(title);
                    keys.Add(field.Name);
                }
            }
        }
    }
}