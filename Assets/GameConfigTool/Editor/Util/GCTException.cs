using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GCT
{
    internal class GCTException : Exception
    {
        public GCTExcel Excel { get; private set; }
        public GCTException(GCTExcel excel, string message)
            : base(message)
        {
            Excel = excel;
        }
    }

    internal class PCall
    {
        public static Action<T> Call<T>(Action<T> action)
        {
            return (T param) =>
            {
                try
                {
                    action(param);
                }
                catch (GCTException e)
                {
                    Debugger.LogError(string.Format("Excel:{0} 错误:{1}", e.Excel.name, e.Message));
                }
                catch (Exception e)
                {
                    Debugger.LogException(e);
                }
            };
        }
    }
}

