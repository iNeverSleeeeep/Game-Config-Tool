using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

namespace GCT
{
    internal class ProtoHeader
    {
        private static List<string> ImportList = new List<string>()
        {
            "keywords.proto",
            "commontypes.proto",
            "excelkeys.proto",
        };
        public static string Package = "Config";

        private static string header;
        public static string Get()
        {
            return header;
        }

        public static void CopyInclude()
        {
            FileHelper.MakeSureDirectory(GCTSettings.Instance.OutputPath + "/proto/");
            foreach (var import in ImportList)
            {
                var from = GCTSettings.Instance.IncludePath + "/" + import;
                var to = GCTSettings.Instance.OutputPath + "/proto/" + import;
                if (File.Exists(from))
                    FileHelper.Copy(from, to);
            }
        }

        public static void Generate()
        {
            var sb = new StringBuilder();
            sb.AppendLine("syntax = \"proto2\";");
            sb.AppendLine();
            foreach (var import in ImportList)
                sb.AppendFormat("import \"{0}\";\n", import);
            sb.AppendLine();
            sb.AppendFormat("package {0};\n", Package);
            sb.AppendLine();
            header = sb.ToString();
        }
    }
}
