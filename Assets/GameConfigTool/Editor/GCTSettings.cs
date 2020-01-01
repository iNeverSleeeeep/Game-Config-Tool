using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GCT
{
    [CreateAssetMenu(fileName = "GCTSettings", menuName = "Fusion/GCTSettings", order = -3)]
    public class GCTSettings : ScriptableObject
    {
        private static GCTSettings _instnce;
        internal static GCTSettings Instance
        {
            get
            {
                if (_instnce == null)
                    _instnce = Resources.Load("GCTSettings") as GCTSettings;
                return _instnce;
            }
        }

        public string ExcelPath;
        public string OutputPath;
        public string IncludePath;
        public string Protoc;
        public string LuaRequirePath;
    }
}

