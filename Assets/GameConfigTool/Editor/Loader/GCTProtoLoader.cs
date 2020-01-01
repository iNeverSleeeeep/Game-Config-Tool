using System;
using System.Collections.Generic;
using System.IO;
using NPOI.XSSF.UserModel;
using UnityEngine;
using System.Diagnostics;

namespace GCT
{
    internal class GCTProtoLoader
    {
        private static object locker = new object();
        private static IDictionary<string, string> Result;
        public static IDictionary<string, string> Load(IEnumerable<string> paths)
        {
            var sw = new Stopwatch();
            sw.Start();
            Result = new SortedDictionary<string, string>();
            Parallel.ForEach(paths, PCall.Call<string>(LoadThread));
            sw.Stop();
            Debugger.Log(string.Format("全部Excel加载完成，耗时{0:N2}秒", (float)sw.ElapsedMilliseconds / 1000));
            return Result;
        }

        private static void LoadThread(string path)
        {
            var sw = new Stopwatch();
            sw.Start();

            sw.Stop();
            Debugger.Log(string.Format("加载{0}完成，耗时{1:N2}秒", path, (float)sw.ElapsedMilliseconds / 1000));
        }
    }
}
