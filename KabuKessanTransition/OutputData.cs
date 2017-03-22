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
        public Kabuka LatestKabuka { get; set; }
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

                ret.LatestKabuka = getLatestDateData(targetCodeList);
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
                nhpp = ((k.NextDayKabuka.HighPrice - k.ReferenceDayKabuka.Price) / k.ReferenceDayKabuka.Price)?.ToString("p2");
            }

            //2週間前
            string twbd = k.TwoWeeksBeforeKabuka.Date.ToString("yyyy/MM/dd");
            string twbp = k.TwoWeeksBeforeKabuka.Price.ToString();
            string twbpp = "";
            if (k.ReferenceDayKabuka.Price != null && k.TwoWeeksBeforeKabuka.Price != null)
            {
                twbpp = ((k.TwoWeeksBeforeKabuka.Price - k.ReferenceDayKabuka.Price) / k.ReferenceDayKabuka.Price)?.ToString("p2");
            }

            //2週間前から基準日までの最大
            string twbhd = k.TwoWeeksBeforeHighLow.HighestDate?.ToString("yyyy/MM/dd");
            string twbhp = k.TwoWeeksBeforeHighLow.HighestPrice.ToString();
            string twbhpp = "";
            if (k.ReferenceDayKabuka.Price != null && k.TwoWeeksBeforeHighLow.HighestPrice != null)
            {
                twbhpp = ((k.TwoWeeksBeforeHighLow.HighestPrice - k.ReferenceDayKabuka.Price) / k.ReferenceDayKabuka.Price)?.ToString("p2");
            }

            //2週間前から基準日までの最小
            string twbld = k.TwoWeeksBeforeHighLow.LowestDate?.ToString("yyyy/MM/dd");
            string twblp = k.TwoWeeksBeforeHighLow.LowestPrice.ToString();
            string twblpp = "";
            if (k.ReferenceDayKabuka.Price != null && k.TwoWeeksBeforeHighLow.LowestPrice != null)
            {
                twblpp = ((k.TwoWeeksBeforeHighLow.LowestPrice - k.ReferenceDayKabuka.Price) / k.ReferenceDayKabuka.Price)?.ToString("p2");
            }

            //2週間後
            string twad = k.TwoWeeksAfterKabuka.Date.ToString("yyyy/MM/dd");
            string twap = k.TwoWeeksAfterKabuka.Price.ToString();
            string twapp = "";
            if (k.ReferenceDayKabuka.Price != null && k.TwoWeeksAfterKabuka.Price != null)
            {
                twapp = ((k.TwoWeeksAfterKabuka.Price - k.ReferenceDayKabuka.Price) / k.ReferenceDayKabuka.Price)?.ToString("p2");
            }

            //基準日から2週間後までの最大
            string twahd = k.TwoWeeksAfterHighLow.HighestDate?.ToString("yyyy/MM/dd");
            string twahp = k.TwoWeeksAfterHighLow.HighestPrice.ToString();
            string twahpp = "";
            if (k.ReferenceDayKabuka.Price != null && k.TwoWeeksAfterHighLow.HighestPrice != null)
            {
                twahpp = ((k.TwoWeeksAfterHighLow.HighestPrice - k.ReferenceDayKabuka.Price) / k.ReferenceDayKabuka.Price)?.ToString("p2");
            }

            //基準日から2週間後までの最小
            string twald = k.TwoWeeksAfterHighLow.LowestDate?.ToString("yyyy/MM/dd");
            string twalp = k.TwoWeeksAfterHighLow.LowestPrice.ToString();
            string twalpp = "";
            if (k.ReferenceDayKabuka.Price != null && k.TwoWeeksAfterHighLow.LowestPrice != null)
            {
                twalpp = ((k.TwoWeeksAfterHighLow.LowestPrice - k.ReferenceDayKabuka.Price) / k.ReferenceDayKabuka.Price)?.ToString("p2");
            }

            //最新株価
            string ld = k.LatestKabuka.Date.ToString("yyyy/MM/dd"); ;
            string lp = k.LatestKabuka.Price.ToString();
            string lpp = ((k.LatestKabuka.Price - k.ReferenceDayKabuka.Price) / k.ReferenceDayKabuka.Price)?.ToString("p2");

            //基準日を起点にした動き
            string sb = createSparkLineBefore(k.ReferenceDayKabuka,k.TwoWeeksBeforeHighLow);
            string sa = createSparkLineAfter(k.ReferenceDayKabuka,k.TwoWeeksAfterHighLow,k.NextDayKabuka,k.LatestKabuka);



            string[] ret = { n,twbhd,twbhp,twbhpp,twbld,twblp,twblpp,sb, rd, rp, nd,nhp,nhpp,twad,twahd,twahp,twahpp,twald,twalp,twalpp ,ld,lp,lpp, sa };
            return string.Join("\t", ret);
        }

        private IEnumerable<CsvKabukaRecode> getTargetCodeList(string targetCode)
        {

            return from p in srcKabukaList
                   where p.Code == targetCode
                   select p;
        }


        const string SparkLineFormat2 = "=SPARKLINE({{{0},{1}}},{{\"charttype\",\"column\";\"color\",\"orange\";\"ymin\",{2}}})";
        const string SparkLineFormat3 = "=SPARKLINE({{{0},{1},{2}}},{{\"charttype\",\"column\";\"color\",\"orange\";\"highcolor\",\"red\";\"ymin\",{3}}})";
        const string SparkLineFormat4 = "=SPARKLINE({{{0},{1},{2},{3}}},{{\"charttype\",\"column\";\"color\",\"orange\";\"highcolor\",\"red\";\"ymin\",{4}}})";
        const string SparkLineFormat5 = "=SPARKLINE({{{0},{1},{2},{3},{4}}},{{\"charttype\",\"column\";\"color\",\"orange\";\"highcolor\",\"red\";\"ymin\",{5}}})";

        private string createSparkLineBefore(Kabuka rk, KabukaHighLow hl)
        {
            var retVal = "";

            if (rk.Price == null || hl.LowestPrice == null || hl.HighestPrice == null)
            {
                return retVal;
            }

            double[] ps = { hl.HighestPrice.GetValueOrDefault(0.0), hl.LowestPrice.GetValueOrDefault(0.0), rk.Price.GetValueOrDefault(0.0) };
            string graphMin = Math.Round((ps.Min() * 0.95)).ToString();

            if (hl.HighestDate > hl.LowestDate)
            {
                retVal = String.Format(SparkLineFormat3, hl.LowestPrice, hl.HighestPrice, rk.Price,  graphMin);
            }
            else if (hl.HighestDate < hl.LowestDate)
            {
                retVal = String.Format(SparkLineFormat3, hl.HighestPrice, hl.LowestPrice, rk.Price,  graphMin);
            }
            else
            {
                retVal = String.Format(SparkLineFormat2, (hl.HighestPrice + hl.LowestPrice)/2, rk.Price, graphMin);
            }

            return retVal;

        }

        private string createSparkLineAfter(Kabuka rk, KabukaHighLow hl,Kabuka nk,Kabuka lk)
        {
            var retVal = "";

            if (rk.Price == null || hl.LowestPrice == null || hl.HighestPrice == null || nk.HighPrice==null||lk.Price==null)
            {
                return retVal;
            }

            double[] ps = { hl.HighestPrice.GetValueOrDefault(0.0), hl.LowestPrice.GetValueOrDefault(0.0), rk.Price.GetValueOrDefault(0.0),nk.HighPrice.GetValueOrDefault(0.0),lk.Price.GetValueOrDefault(0.0) };
            string graphMin = Math.Round((ps.Min() * 0.95)).ToString();

            if (hl.HighestDate > hl.LowestDate)
            {
                retVal = String.Format(SparkLineFormat5, rk.Price, nk.HighPrice, hl.LowestPrice, hl.HighestPrice, lk.Price, graphMin);
            }
            else if (hl.HighestDate < hl.LowestDate)
            {
                retVal = String.Format(SparkLineFormat5, rk.Price, nk.HighPrice, hl.HighestPrice, hl.LowestPrice, lk.Price, graphMin);
            }
            //最新日が翌日の場合は意味が無いから削除
            else if (nk.Date == lk.Date)
            {
                return retVal;
            } else
            {
                retVal = String.Format(SparkLineFormat4, rk.Price, nk.HighPrice, (hl.HighestPrice + hl.LowestPrice) / 2,lk.Price,  graphMin);
            }

            return retVal;

        }

        private Kabuka getLatestDateData(IEnumerable<CsvKabukaRecode> targetList)
        {
            var targetDateKabuka = (from p in targetList
                                    orderby p.PriceDate descending
                                    select p).FirstOrDefault();

            if (targetDateKabuka != null) {
                return new Kabuka(targetDateKabuka.PriceDate, targetDateKabuka.EndPrice, targetDateKabuka.HighPrice, targetDateKabuka.HighPrice);
            }
            else {
                return null;
            }

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
