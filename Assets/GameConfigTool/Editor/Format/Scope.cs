using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GCT
{
    public class Scope : IDisposable
    {
        private StringBuilder m_StringBuilder;
        private Indent m_Indent;
        private string m_End;
        public Scope(StringBuilder sb, string title, Indent indent, string end = "")
        {
            m_Indent = indent;
            m_StringBuilder = sb;
            m_End = end;
            
            m_StringBuilder.Append(m_Indent.Format("{0} {{\n", title));
            indent++;
        }

        public void Dispose()
        {
            m_Indent--;
            m_StringBuilder.Append(m_Indent.Format("}}{0}\n", m_End));
        }
    }
}