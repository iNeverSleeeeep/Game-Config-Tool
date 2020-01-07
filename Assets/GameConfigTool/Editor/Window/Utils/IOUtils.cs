using System;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading;
using UnityEditor.VersionControl;

namespace GCT.Window
{
    internal static class IOUtils
    {
        public static string MainSkinGUID = "941081ea698d79d4a9e64b6b1c930e1f";

        public static Dictionary<string, GCTWindow> AllOpenedWindows = new Dictionary<string, GCTWindow>();

        public static void SaveTextfileToDisk(string shaderBody, string pathName)
        {
            // Write to disk
            StreamWriter fileWriter = new StreamWriter(pathName);
            try
            {
                fileWriter.Write(shaderBody);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                fileWriter.Close();
            }
        }
    }

}
