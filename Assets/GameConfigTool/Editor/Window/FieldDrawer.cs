using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GCT
{
    internal static class FieldDrawer
    {
        public static void Draw(this GCTField field, GCTRowTable row, int fieldIndex)
        {
            var titles = ListPool<string>.Get();
            if (row.FieldTitles.Count > fieldIndex)
            {
                var titleCells = row.FieldTitles[fieldIndex];
                for (var i = 0; i < titleCells.Count; ++i)
                    titles.Add(titleCells[i].Value());
            }

            if (row.FieldCells.Count > fieldIndex)
                field.Type.Draw(row.FieldCells[fieldIndex], titles);

            ListPool<string>.Release(titles);
        }
    }
}
