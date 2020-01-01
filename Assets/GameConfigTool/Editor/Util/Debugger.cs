using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GCT
{
    internal enum LogBehaviour
    {
        /// <summary>Log only warnings and errors</summary>
        Default,
        /// <summary>Log warnings, errors and additional infos</summary>
        Verbose,
        /// <summary>Log only errors</summary>
        ErrorsOnly
    }

    internal static class Debugger
    {
        private static int logPriority;

        public static void Log(object message)
        {
            if (logPriority == 2)
                Debug.Log((object)("<color=#0099bc><b>导表工具 ► </b></color>" + message));
        }

        public static void LogInfo(object message)
        {
            Debug.Log((object)("<color=#0099bc><b>导表工具 ► </b></color>" + message));
        }

        public static void LogWarning(object message)
        {
            if (logPriority != 0)
                Debug.LogWarning((object)("<color=#0099bc><b>导表工具 ► </b></color>" + message));
        }

        public static void LogError(object message)
        {
            Debug.LogError((object)("<color=#0099bc><b>导表工具 ► </b></color>" + message));
        }

        public static void LogException(Exception e)
        {
            Debug.LogException(e);
        }

        public static void SetLogPriority(LogBehaviour logBehaviour)
        {
            switch (logBehaviour)
            {
                case LogBehaviour.Default:
                    logPriority = 1;
                    break;
                case LogBehaviour.Verbose:
                    logPriority = 2;
                    break;
                default:
                    logPriority = 0;
                    break;
            }
        }
    }
}
