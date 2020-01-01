using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GCT
{
    public static class ICellExtensions 
    {
        public static string Value(this ICell cell)
        {
            switch (cell.CellType)
            {
                case CellType.Unknown:
                    return string.Empty;
                case CellType.Numeric:
                    return cell.NumericCellValue.ToString();
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Formula:
                    return string.Empty;
                case CellType.Blank:
                    return string.Empty;
                case CellType.Boolean:
                    return string.Empty;
                case CellType.Error:
                    return string.Empty;
            }
            return string.Empty;
        }

        public static object RawValue(this ICell cell)
        {
            switch (cell.CellType)
            {
                case CellType.Unknown:
                    return string.Empty;
                case CellType.Numeric:
                    return cell.NumericCellValue;
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Formula:
                    return string.Empty;
                case CellType.Blank:
                    return string.Empty;
                case CellType.Boolean:
                    return string.Empty;
                case CellType.Error:
                    return string.Empty;
            }
            return string.Empty;
        }
    }
}

