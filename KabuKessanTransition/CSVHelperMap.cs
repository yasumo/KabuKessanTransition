using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KabuKessanTransition
{

    class CsvInputCodeRecode {
        public string Code { get; set; }
        public DateTime Date { get; set; }
        public int Offset { get; set; }
        public Double QuarterEps { get; set; }
    }
    class InputCodeMap : CsvClassMap<CsvInputCodeRecode>
    {
        public InputCodeMap()
        {
            Map(m => m.Code).Index(0);
            Map(m => m.Date).Index(1).TypeConverter<CsvDateConverter2>();
            Map(m => m.Offset).Index(2).TypeConverter<CsvIntConverter>();
            Map(m => m.QuarterEps).Index(3).TypeConverter<CsvDoubleConverter>();
        }
    }

    class StockPriceCSVRecode
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Double EndPrice { get; set; }
        public Double HighPrice { get; set; }
        public Double LowPrice { get; set; }
        public DateTime PriceDate { get; set; }
    }

    class StockPriceMap : CsvClassMap<StockPriceCSVRecode>
    {
        public StockPriceMap()
        {
            Map(m => m.Code).Index(0);
            Map(m => m.Name).Index(1);
            Map(m => m.EndPrice).Index(5).TypeConverter<CsvDoubleConverter>();
            Map(m => m.HighPrice).Index(10).TypeConverter<CsvDoubleConverter>();
            Map(m => m.LowPrice).Index(11).TypeConverter<CsvDoubleConverter>();
            Map(m => m.PriceDate).Index(4).TypeConverter<CsvDateConverter>();
        }
    }

    class StockDataCSVRecode
    {
        public string Code { get; set; }
        public int Capitalization { get; set; }
        public Double Per { get; set; }
        public Double Eps { get; set; }
    }

    class StockDataMap : CsvClassMap<StockDataCSVRecode>
    {
        public StockDataMap()
        {
            Map(m => m.Code).Index(0);
            Map(m => m.Capitalization).Index(4).TypeConverter<CsvIntConverter>();
            Map(m => m.Per).Index(8).TypeConverter<CsvDoubleConverter>();
            Map(m => m.Eps).Index(10).TypeConverter<CsvDoubleConverter>();
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

    public class CsvDateConverter2 : CsvHelper.TypeConversion.DateTimeConverter
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
            return DateTime.ParseExact(text, "yyyy/MM/dd", null);
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

    public class CsvIntConverter : CsvHelper.TypeConversion.DateTimeConverter
    {
        public override object ConvertFromString(CsvHelper.TypeConversion.TypeConverterOptions options, string text)
        {
            int ret = 0;
            if (text == null)
            {
                return ret;
            }

            if (text.Trim().Length == 0 || text == "-")
            {
                return ret;
            }

            Int32.TryParse(text, out ret);
            return ret;

        }
    }

}
