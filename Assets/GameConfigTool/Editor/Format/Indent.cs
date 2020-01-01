using System;
using System.Collections.Generic;
using System.Threading;

namespace GCT
{
    public class Indent
    {
        private string m_Blank = "";
        private int m_Level = 0;

        public int Level
        {
            get
            {
                return m_Level;
            }
            private set
            {
                if (value < 0)
                {
                    Debugger.LogError("缩进错误");
                    return;
                }
                m_Level = value;
                if (m_Level == 0)
                    m_Blank = string.Empty;
                else
                    m_Blank = new string(' ', m_Level * 4);
            }
        }

        public void Reset()
        {
            Level = 0;
        }

        public string Format(string format)
        {
            return m_Blank + format;
        }
        public string Format(string format, params object[] args)
        {
            return string.Format(m_Blank + format, args);
        }
        public string Format(string format, object arg0, object arg1, object arg2)
        {
            return string.Format(m_Blank + format, arg0, arg1, arg2);
        }
        public string Format(string format, object arg0, object arg1)
        {
            return string.Format(m_Blank + format, arg0, arg1);
        }
        public string Format(string format, object arg0)
        {
            return string.Format(m_Blank + format, arg0);
        }

        public override string ToString()
        {
            return m_Blank;
        }

        public static Indent operator ++(Indent a)
        {
            a.Level++;
            return a;
        }

        public static Indent operator --(Indent a)
        {
            a.Level--;
            return a;
        }
    }
}