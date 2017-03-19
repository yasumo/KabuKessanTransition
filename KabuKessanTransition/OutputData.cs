using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KabuKessanTransition
{
    class Kabuka {
        public DateTime Date { get; set; }
        public Double? Price { get; set; }
        public Double? HighPrice { get; set; }
        public Double? LowPrice { get; set; }
        public Kabuka(DateTime date, double? price, double? highPrice, double? lowPrice)
        {
            Date = date;
            Price = price;
            HighPrice = highPrice;
            LowPrice = lowPrice;
        }
    }
    class KabukaHighLow
    {
        public DateTime? HighestDate { get; set; }
        public Double? HighestPrice { get; set; }
        public DateTime? LowestDate { get; set; }
        public Double? LowestPrice { get; set; }
        public KabukaHighLow(DateTime? highestDate, Double? highestPrice, DateTime? lowestDate, Double? lowestPrice)
        {
            HighestDate = highestDate;
            HighestPrice = highestPrice;
            LowestDate = lowestDate;
            LowestPrice = lowestPrice;
        }
    }

    class OutputKabuka
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Kabuka ReferenceDayKabuka { get; set; }
        public Kabuka NextDayKabuka { get; set; }
        public Kabuka TwoWeeksBeforeKabuka { get; set; }
        public Kabuka TwoWeeksAfterKabuka { get; set; }
        public KabukaHighLow TwoWeeksBeforeHighLow { get; set; }
        public KabukaHighLow TwoWeeksAfterHighLow { get; set; }

        public OutputKabuka(string code)
        {
            Code = code;
        }

    }
    class OutputDataService
    {
        private List<CsvKabukaRecode> srcKabukaList;

        public OutputDataService(List<CsvKabukaRecode> kabukaList) {
            if (kabukaList == null || kabukaList.Count() == 0)
            {
                throw new ArgumentNullException("csvデータが空です");
            }

            srcKabukaList = kabukaList;
        }

        public OutputKabuka SearchKabuka(string targetCode, DateTime ReferenceDate)
        {
            if (targetCode == null || targetCode.Length == 0 || ReferenceDate == null)
            {
                throw new ArgumentNullException("引数がおかしいです");
            }

            var ret = new OutputKabuka(targetCode);

            //ターゲットの銘柄コードだけを抽出
            var targetCodeList = getTargetCodeList(ret.Code);
            //銘柄コードのデータが無ければ終了
            if (targetCodeList == null || targetCodeList.Count() == 0)
            {
                return ret;
            }

            //ターゲットの名前取得
            ret.Name = targetCodeList.First().Name;

            {
                TimeSpan ts1 = new TimeSpan(1, 0, 0, 0);
                TimeSpan ts13 = new TimeSpan(13, 0, 0, 0);
                TimeSpan ts14 = new TimeSpan(14, 0, 0, 0);

                ret.ReferenceDayKabuka = getTargetDateData(targetCodeList, ReferenceDate);
                ret.TwoWeeksBeforeKabuka = getTargetDateData(targetCodeList, ret.ReferenceDayKabuka.Date - ts14);

                ret.NextDayKabuka = getTargetNextDateData(targetCodeList, ret.ReferenceDayKabuka.Date);
                ret.TwoWeeksAfterKabuka = getTargetNextDateData(targetCodeList, ret.ReferenceDayKabuka.Date + ts13);


                ret.TwoWeeksAfterHighLow = getHighLowPrice(targetCodeList, ret.ReferenceDayKabuka.Date+ts1, ret.ReferenceDayKabuka.Date + ts14);
                ret.TwoWeeksBeforeHighLow = getHighLowPrice(targetCodeList, ret.ReferenceDayKabuka.Date - ts14,ret.ReferenceDayKabuka.Date );
            }

            return ret;
        }
        public string OutputTsv(OutputKabuka kabukaRecode)
        {
            var k = kabukaRecode;
            //名前
            string n = k.Name;
            //基準日
            string rd = k.ReferenceDayKabuka.Date.ToString("yyyy/MM/dd");
            string rp = k.ReferenceDayKabuka.Price.ToString();

            //基準日の次の日
            string nd = k.NextDayKabuka.Date.ToString("yyyy/MM/dd");
            string nhp = k.NextDayKabuka.HighPrice.ToString();
            string nhpp = "";
            if (k.NextDayKabuka.Price != null && k.ReferenceDayKabuka.Price != null)
            {
                nhpp = ((k.NextDayKabuka.Price - k.ReferenceDayKabuka.Price) / k.ReferenceDayKabuka.Price )?.ToString("p2");
            }

            //2週間前
            string twbd = k.TwoWeeksBeforeKabuka.Date.ToString("yyyy/MM/dd");
            string twbp = k.TwoWeeksBeforeKabuka.Price.ToString();
            string twbpp = "";
            if (k.ReferenceDayKabuka.Price != null && k.TwoWeeksBeforeKabuka.Price != null)
            {
                twbpp = ((k.ReferenceDayKabuka.Price - k.TwoWeeksBeforeKabuka.Price) / k.TwoWeeksBeforeKabuka.Price )?.ToString("p2");
            }

            //2週間前から基準日までの最大
            string twbhd = k.TwoWeeksBeforeHighLow.HighestDate?.ToString("yyyy/MM/dd");
            string twbhp = k.TwoWeeksBeforeHighLow.HighestPrice.ToString();
            string twbhpp = "";
            if (k.ReferenceDayKabuka.Price != null && k.TwoWeeksBeforeHighLow.HighestPrice != null)
            {
                twbhpp = ((k.ReferenceDayKabuka.Price - k.TwoWeeksBeforeHighLow.HighestPrice) / k.TwoWeeksBeforeHighLow.HighestPrice )?.ToString("p2");
            }

            //2週間前から基準日までの最小
            string twbld = k.TwoWeeksBeforeHighLow.LowestDate?.ToString("yyyy/MM/dd");
            string twblp = k.TwoWeeksBeforeHighLow.LowestPrice.ToString();
            string twblpp = "";
            if (k.ReferenceDayKabuka.Price != null && k.TwoWeeksBeforeHighLow.LowestPrice != null)
            {
                twblpp = ((k.ReferenceDayKabuka.Price - k.TwoWeeksBeforeHighLow.LowestPrice) / k.TwoWeeksBeforeHighLow.LowestPrice )?.ToString("p2");
            }

            //2週間後
            string twad = k.TwoWeeksAfterKabuka.Date.ToString("yyyy/MM/dd");
            string twap = k.TwoWeeksAfterKabuka.Price.ToString();
            string twapp = "";
            if (k.ReferenceDayKabuka.Price != null && k.TwoWeeksAfterKabuka.Price != null)
            {
                twapp = ((k.TwoWeeksAfterKabuka.Price - k.ReferenceDayKabuka.Price) / k.ReferenceDayKabuka.Price )?.ToString("p2");
            }

            //基準日から2週間後までの最大
            string twahd = k.TwoWeeksAfterHighLow.HighestDate?.ToString("yyyy/MM/dd");
            string twahp = k.TwoWeeksAfterHighLow.HighestPrice.ToString();
            string twahpp = "";
            if (k.ReferenceDayKabuka.Price != null && k.TwoWeeksAfterHighLow.HighestPrice != null)
            {
                twahpp = ((k.TwoWeeksAfterHighLow.HighestPrice - k.ReferenceDayKabuka.Price) / k.ReferenceDayKabuka.Price )?.ToString("p2");
            }

            //基準日から2週間後までの最小
            string twald = k.TwoWeeksAfterHighLow.LowestDate?.ToString("yyyy/MM/dd");
            string twalp = k.TwoWeeksAfterHighLow.LowestPrice.ToString();
            string twalpp = "";
            if (k.ReferenceDayKabuka.Price != null && k.TwoWeeksAfterHighLow.LowestPrice != null)
            {
                twalpp = ((k.TwoWeeksAfterHighLow.LowestPrice - k.ReferenceDayKabuka.Price) / k.ReferenceDayKabuka.Price )?.ToString("p2");
            }

            string[] ret = { n,rd,rp,twbd,twbp,twbpp,twbhd,twbhp,twbhpp,twbld,twblp,twblpp,nd,nhp,nhpp,twad,twahd,twahp,twahpp,twald,twalp,twalpp };
            return string.Join("\t", ret);
        }

        private IEnumerable<CsvKabukaRecode> getTargetCodeList(string targetCode)
        {

            return from p in srcKabukaList
                   where p.Code == targetCode
                   select p;
        }


        private Kabuka getTargetDateData(IEnumerable<CsvKabukaRecode> targetList,DateTime targetDate)
        {
            DateTime retDate=targetDate;
            Double? retPrice = null;
            Double? retHighPrice = null;
            Double? retLowPrice = null;

            var targetDateKabuka = (from p in targetList
                               where p.PriceDate.Date <= targetDate.Date
                               orderby p.PriceDate descending
                               select p).FirstOrDefault();

            if (targetDateKabuka != null)
            {
                retDate = targetDateKabuka.PriceDate;
                retPrice = targetDateKabuka.EndPrice;
                retHighPrice = targetDateKabuka.HighPrice;
                retLowPrice = targetDateKabuka.LowPrice;
            }

            return new Kabuka(retDate, retPrice,retHighPrice,retLowPrice);
        }
        private Kabuka getTargetNextDateData(IEnumerable<CsvKabukaRecode> targetList, DateTime targetDate)
        {
            DateTime retDate = targetDate;
            Double? retPrice = null;
            Double? retHighPrice = null;
            Double? retLowPrice = null;

            var targetNextDateKabuka = (from p in targetList
                                    where p.PriceDate.Date > targetDate.Date
                                    orderby p.PriceDate ascending
                                    select p).FirstOrDefault();

            if (targetNextDateKabuka != null)
            {
                retDate = targetNextDateKabuka.PriceDate;
                retPrice = targetNextDateKabuka.EndPrice;
                retHighPrice = targetNextDateKabuka.HighPrice;
                retLowPrice = targetNextDateKabuka.LowPrice;
            }

            return new Kabuka(retDate, retPrice,retHighPrice, retLowPrice);
        }

        private KabukaHighLow getHighLowPrice(IEnumerable<CsvKabukaRecode> targetList, DateTime start,DateTime end)
        {
            DateTime? retHighDate = null;
            Double? retHighPrice = null;
            DateTime? retLowDate = null;
            Double? retLowPrice = null;
            var kabukaHighest = (from p in targetList
                            where p.PriceDate >= start & p.PriceDate <= end
                            orderby p.HighPrice descending
                            select p).FirstOrDefault();

            if (kabukaHighest != null)
            {
                retHighDate = kabukaHighest.PriceDate;
                retHighPrice = kabukaHighest.HighPrice;
            }

            var kabukaLowest = (from p in targetList
                            where p.PriceDate >= start & p.PriceDate <= end
                            orderby p.LowPrice ascending
                            select p).FirstOrDefault();
            if (kabukaLowest != null)
            {
                retLowDate = kabukaLowest.PriceDate;
                retLowPrice = kabukaLowest.LowPrice;
            }
            return new KabukaHighLow(retHighDate,retHighPrice,retLowDate, retLowPrice);
        }
    }

}
