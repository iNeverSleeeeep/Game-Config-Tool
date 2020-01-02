using System;
using System.Collections.Generic;
using System.IO;
using NPOI.XSSF.UserModel;
using UnityEngine;
using System.Diagnostics;
using NPOI.OpenXml4Net.OPC;

namespace GCT
{
    internal class GCTExcelLoader
    {
        private static object locker = new object();
        public static SortedDictionary<string, GCTExcel> Excels;

        // 子Excel的命名为aa(bb).xlsx bb为任意字符 可以为数字分组
        public static SortedDictionary<string, GCTExcel> Load(IEnumerable<string> paths)
        {
            var sw = new Stopwatch();
            sw.Start();
            Excels = new SortedDictionary<string, GCTExcel>();
            var mainexcel = new List<string>();
            var subexcel = new List<string>();

            foreach (var path in paths)
            {
                if (path.Contains("("))
                    subexcel.Add(path);
                else if (path.Contains("~$")) // 临时文件
                    continue;
                else
                    mainexcel.Add(path);
            }

            Parallel.ForEach(mainexcel, PCall.Call<string>(LoadThread));
            Parallel.ForEach(subexcel, PCall.Call<string>(LoadThread));
            sw.Stop();
            Debugger.Log(string.Format("全部Excel加载完成，耗时{0:N2}秒", (float)sw.ElapsedMilliseconds / 1000));
            return Excels;
        }

        public static GCTExcel GetExcel(string name)
        {
            if (Excels.ContainsKey(name))
                return Excels[name];
            else
                throw new Exception("excel 不存在:" + name);
        }

        public static void GenerateData(IEnumerable<GCTExcel> excels)
        {
            var sw = new Stopwatch();
            sw.Start();
            Parallel.ForEach(excels, PCall.Call<GCTExcel>(GenerateDataThread));
            Parallel.ForEach(excels, PCall.Call<GCTExcel>(CheckDataThread));
            sw.Stop();
            Debugger.Log(string.Format("全部ExcelData加载完成，耗时{0:N2}秒", (float)sw.ElapsedMilliseconds / 1000));
        }

        private static void LoadThread(string path)
        {
            GCTExcel excel = null;
            var sw = new Stopwatch();
            sw.Start();
            FileStream fs = null;
            bool isTempPath = false;
            try
            {
                fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            } catch
            {
            }

            if (fs == null)
            {
                File.Copy(path, path + ".tmp.xlsx");
                fs = new FileStream(path + ".tmp.xlsx", FileMode.Open, FileAccess.Read, FileShare.Read);
                isTempPath = true;
            }

            if (fs == null)
            {
                Debugger.LogError("打开文件失败:" + Path.GetFileName(path));
                return;
            }
            var workbook = new XSSFWorkbook(fs);
            if (path.Contains("("))
            {
                var name = Path.GetFileNameWithoutExtension(path);
                name = name.Substring(0, name.IndexOf('('));
                var datasheet = workbook.GetSheet("data");
                lock (locker)
                {
                    Excels[name].SubDataSheet.Add(datasheet);
                }
            }
            else
            {
                excel = new GCTExcel();
                excel.name = Path.GetFileNameWithoutExtension(path);
                excel.path = path;
                excel.DataSheet = workbook.GetSheet("data");
                excel.SchemaSheet = workbook.GetSheet("schema");
                excel.Schema = new GCTSchema(excel.SchemaSheet, excel.DataSheet.GetRow(0));
                excel.ConfigSheet = workbook.GetSheet("config");
                excel.Config = new GCTConfig(excel.ConfigSheet, excel);
            }

            if (fs != null)
                fs.Dispose();
            if (isTempPath)
                File.Delete(path + ".tmp.xlsx");

            sw.Stop();
            Debugger.Log(string.Format("加载{0}完成，耗时{1:N2}秒", Path.GetFileName(path), (float)sw.ElapsedMilliseconds / 1000));

            if (excel != null)
            {
                lock (locker)
                {
                    Excels.Add(excel.name, excel);
                }
            }
        }

        private static void GenerateDataThread(GCTExcel excel)
        {
            var sw = new Stopwatch();
            sw.Start();

            excel.Data = new GCTData(excel);

            sw.Stop();
            Debugger.Log(string.Format("加载{0}Data完成，耗时{1:N2}秒", excel.name, (float)sw.ElapsedMilliseconds / 1000));
        }

        private static void CheckDataThread(GCTExcel excel)
        {
            var sw = new Stopwatch();
            sw.Start();

            excel.Data.Check();

            sw.Stop();
            Debugger.Log(string.Format("加载{0}Data完成，耗时{1:N2}秒", excel.name, (float)sw.ElapsedMilliseconds / 1000));
        }
    }
}
