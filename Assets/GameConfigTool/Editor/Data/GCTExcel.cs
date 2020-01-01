using NPOI.SS.UserModel;
using System.Collections.Generic;

namespace GCT
{
    internal class GCTExcel
    {
        public string name;
        public string path;
        public ISheet DataSheet;
        public List<ISheet> SubDataSheet = new List<ISheet>();
        public GCTData Data;
        public ISheet SchemaSheet;
        public GCTSchema Schema;
        public ISheet ConfigSheet;
        public GCTConfig Config;

        public string md5;
    }
}