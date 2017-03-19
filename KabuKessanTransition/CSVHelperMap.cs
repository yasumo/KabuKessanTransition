using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KabuKessanTransition
{
    class Kabuka
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string EndPrice { get; set; }
        public string HighPrice { get; set; }
        public string LowPrice { get; set; }
    }

    class KabukaMap : CsvClassMap<Kabuka>
    {
        public KabukaMap()
        {
            Map(m => m.Code).Index(0);
            Map(m => m.Name).Index(1);
            Map(m => m.EndPrice).Index(5);
            Map(m => m.HighPrice).Index(10);
            Map(m => m.LowPrice).Index(11);
        }
    }

}
