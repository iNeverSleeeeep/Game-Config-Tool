using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GCT
{
    internal class GCTField
    {
        private string m_Title;
        public string Title { get { return m_Title; } }

        private string m_Name;
        public string Name { get { return m_Name; } set { SetHelper(value, ref m_Name); } }

        private GCTType m_Type;
        public GCTType Type { get { return m_Type; } set { SetHelper(value, ref m_Type); } }

        private bool m_IsKey;
        public bool IsKey { get { return m_IsKey; } set { m_IsKey = value; } }

        private bool m_IsOptional;
        public bool IsOptional { get { return m_IsOptional; } set { m_IsOptional = value; } }

        public bool IsArray { get { return m_Type is GCTTypeArray; } }

        private bool m_IsClient;
        public bool IsClient
        {
            get
            {
                if (m_IsKey)
                    return true;
                return m_IsClient;
            }
            set
            {
                m_IsClient = value;
            }
        }

        private bool m_IsServer;
        public bool IsServer
        {
            get
            {
                if (m_IsKey)
                    return true;
                if (Type is GCTTypeJson)
                    return false;
                return m_IsServer;
            }
            set
            {
                m_IsServer = value;
            }
        }

        private bool m_IsCommonType;
        public bool IsCommonType { get { return m_Type is GCTTypeCommonTypes; } }
        
        private int m_ColumnCount;
        public int ColumnCount { get { return m_ColumnCount; } set { m_ColumnCount = value; } }

        public GCTField(string title)
        {
            m_Title = title;
        }

        public object Value(List<ICell> cells, List<string> titles)
        {
            var value = Type.ToValue(cells, titles);
            if (IsOptional == false && value == null)
                throw new System.Exception(Title + "不是选填字段:");
            return value;
        }

        private static void SetHelper<T>(T value, ref T to) where T : class
        {
            if (to != null)
                Debugger.LogError("重复设置");
            else if (value == null)
                Debugger.LogError("设置为空");
            else
                to = value;
        }
    }
}
