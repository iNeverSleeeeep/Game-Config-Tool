using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GCT
{
    [CreateAssetMenu(fileName = "GCTSettings", menuName = "Fusion/GCTSettings", order = -3)]
    public class GCTSettings : ScriptableObject
    {
        private static string DataPath;
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
        [SerializeField]
        private string excelPath;
        [SerializeField]
        private string outputPath;
        [SerializeField]
        private string includePath;
        [SerializeField]
        private string protocPath;
        [SerializeField]
        private string luaRequirePath;

        void OnEnable()
        {
            DataPath = Application.dataPath;
        }

        private string m_ExcelPath;
        public string ExcelPath
        {
            get
            {
                m_ExcelPath = excelPath;
                if (Path.IsPathRooted(m_ExcelPath) == false)
                    m_ExcelPath = Path.Combine(DataPath, excelPath);
                return m_ExcelPath;
            }
        }

        private string m_OutputPath;
        public string OutputPath
        {
            get
            {
                m_OutputPath = outputPath;
                if (Path.IsPathRooted(m_OutputPath) == false)
                    m_OutputPath = Path.Combine(DataPath, outputPath);
                return m_OutputPath;
            }
        }

        private string m_IncludePath;
        public string IncludePath
        {
            get
            {
                m_IncludePath = includePath;
                if (Path.IsPathRooted(m_IncludePath) == false)
                    m_IncludePath = Path.Combine(DataPath, includePath);
                return m_IncludePath;
            }
        }

        private string m_Protoc;
        public string Protoc
        {
            get
            {
                m_Protoc = protocPath;
                if (Path.IsPathRooted(m_Protoc) == false)
                    m_Protoc = Path.Combine(DataPath, protocPath);
                return m_Protoc;
            }
        }
        public string LuaRequirePath
        {
            get
            {
                return luaRequirePath;
            }
        }
    }
}

