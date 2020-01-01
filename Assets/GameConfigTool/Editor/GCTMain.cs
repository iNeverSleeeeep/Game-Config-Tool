using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace GCT
{
    public class GCTMain
    {
        [MenuItem("Xiyou/导出全部Excel")]
        static void ExportAllExpcels()
        {
            var sw = new Stopwatch();
            sw.Start();

            GCTKeywords.Init();
            GCTCommonTypes.Init();
            var files = FileHelper.GetFiles(GCTSettings.Instance.ExcelPath, "*.xlsx");
            var excels = GCTExcelLoader.Load(files);
            
            ProtoGenerator.GenerateProto(excels.Values);
            ProtoGenerator.GenerateLua(GCTSettings.Instance.OutputPath + "/proto");
            ProtoGenerator.GenerateCpp(GCTSettings.Instance.OutputPath + "/proto");

            GCTExcelLoader.GenerateData(excels.Values);

            ProtoCpp.Generate(excels.Values);
            ProtoLua.Generate(excels.Values);
            ProtoBytes.Generate(excels.Values);

            ProtoLua.GenerateKeywords();
            ProtoLua.GenerateVersion();
            ProtoCpp.GenerateVersion();

            sw.Stop();
            Debugger.LogInfo(string.Format("导表完成，耗时{0:N2}秒", (float)sw.ElapsedMilliseconds / 1000));
        }
    }
}

