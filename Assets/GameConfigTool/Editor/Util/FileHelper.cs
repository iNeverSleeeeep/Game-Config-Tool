using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace GCT
{
    public class FileHelper
    {
        public static string[] GetFiles(string path, string pattern)
        {
            return Directory.GetFiles(path, pattern);
        }

        public static void CleanDirectory(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            Directory.CreateDirectory(path);
        }

        public static void WriteAllText(string path, string contents)
        {
            WriteAllText(path, contents, new UTF8Encoding(false));
        }

        private static bool CheckByteEquals(byte[] b1, byte[] b2)
        {
            if (b1.Length != b2.Length)
                return false;
            for (var i = 0; i < b1.Length; ++i)
            {
                if (b1[i] != b2[i])
                    return false;
            }
            return true;
        }

        public static void Copy(string from, string to)
        {
            from = from.Replace("\\", "/");
            to = to.Replace("\\", "/");
            MakeSureDirectory(to);
            if (File.Exists(to))
            {
                if (CheckByteEquals(File.ReadAllBytes(from), File.ReadAllBytes(to)))
                    return;
                File.Delete(to);
            }
            File.Copy(from, to);
            Debugger.LogInfo("拷贝了文件：" + Path.GetFileName(from));
        }

        public static void WriteAllBytes(string path, byte[] bytes)
        {
            path = path.Replace("\\", "/");
            MakeSureDirectory(path);
            if (File.Exists(path))
            {
                if (CheckByteEquals(File.ReadAllBytes(path), bytes))
                    return;
                File.Delete(path);
            }
            File.WriteAllBytes(path, bytes);
            Debugger.LogInfo("写入了文件：" + Path.GetFileName(path));
        }

        public static void WriteAllText(string path, string contents, Encoding encoding)
        {
            path = path.Replace("\\", "/");
            MakeSureDirectory(path);
            if (File.Exists(path))
            {
                if (contents == File.ReadAllText(path, encoding))
                    return;
                File.Delete(path);
            }
            File.WriteAllText(path, contents, encoding);
            Debugger.LogInfo("写入了文件：" + Path.GetFileName(path));
        }

        public static string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public static string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public static string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public static void MakeSureDirectory(string path)
        {
            path = path.Replace("\\", "/");
            int start = 0;
            while (true)
            {
                var index = path.IndexOf('/', start);
                if (index < 0)
                    break;
                start = index + 1;
                var directory = path.Substring(0, index);
                if (Directory.Exists(directory) == false)
                    Directory.CreateDirectory(directory);
            }
        }
    }
}
