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
        public Double EndPrice { get; set; }
        public Double HighPrice { get; set; }
        public Double LowPrice { get; set; }
        public DateTime PriceDate { get; set; }
    }

    class KabukaMap : CsvClassMap<Kabuka>
    {
        public KabukaMap()
        {
            Map(m => m.Code).Index(0);
            Map(m => m.Name).Index(1);
            Map(m => m.EndPrice).Index(5).TypeConverter<CsvDoubleConverter>();
            Map(m => m.HighPrice).Index(10).TypeConverter<CsvDoubleConverter>();
            Map(m => m.LowPrice).Index(11).TypeConverter<CsvDoubleConverter>();
            Map(m => m.PriceDate).Index(4).TypeConverter<CsvDateConverter>();
        }
    }


    public class CsvDateConverter : CsvHelper.TypeConversion.DateTimeConverter
    {
        public override object ConvertFromString(CsvHelper.TypeConversion.TypeConverterOptions options, string text)
        {
            if (text == null)
            {
                return base.ConvertFromString(options, null);
            }

            if (text.Trim().Length == 0 || text == "-")
            {
                return DateTime.MinValue;
            }
            return DateTime.ParseExact(text, "yyyy/M/d HH:mm", null);
        }
    }

    public class CsvDoubleConverter : CsvHelper.TypeConversion.DateTimeConverter
    {
        public override object ConvertFromString(CsvHelper.TypeConversion.TypeConverterOptions options, string text)
        {
            Double ret = 0.0;
            if (text == null)
            {
                return 0.0;
            }

            if (text.Trim().Length == 0 || text == "-")
            {
                return 0.0;
            }

            Double.TryParse(text, out ret);
            return ret;

        }
    }

}
