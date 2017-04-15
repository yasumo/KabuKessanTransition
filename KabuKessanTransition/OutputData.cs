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
        public Double QuarterEps { get; set; }
        public Double Eps { get; set; }
        public Double Per { get; set; }
        public int Capitalization { get; set; }
        public Kabuka ReferenceDayKabuka { get; set; }
        public Kabuka NextDayKabuka { get; set; }
        public Kabuka OffsetDaysBeforeKabuka { get; set; }
        public Kabuka OffsetDaysAfterKabuka { get; set; }
        public Kabuka LatestKabuka { get; set; }
        public Double? Yesterday5SMA { get; set; }
        public Double? Latest5SMA { get; set; }
        public Double? Yesterday5SMACustom { get; set; }
        public Double? Latest5SMACustom { get; set; }
        public KabukaHighLow OffsetDaysBeforeHighLow { get; set; }
        public KabukaHighLow OffsetDaysAfterHighLow { get; set; }

        public OutputKabuka(string code,Double quarterEps)
        {
            Code = code;
            QuarterEps = quarterEps;
        }

    }


    class OutputDataService
    {
        private List<StockPriceCSVRecode> srcPriceList;
        private List<StockDataCSVRecode> srcDataList;

        public OutputDataService(List<StockPriceCSVRecode> priceList,List<StockDataCSVRecode> dataList) {
            if (priceList == null || priceList.Count() == 0|| dataList==null||dataList.Count()==0)
            {
                throw new ArgumentNullException("csvデータのどちらかが空です");
            }

            srcPriceList = priceList;
            srcDataList = dataList;
        }

        public OutputKabuka SearchKabuka(string targetCode, DateTime referenceDate,int offset,Double quarterEps)
        {
            if (targetCode == null || targetCode.Length == 0 || referenceDate == null)
            {
                throw new ArgumentNullException("引数がおかしいです");
            }

            var ret = new OutputKabuka(targetCode, quarterEps);

            //ターゲットの銘柄コードの基本情報を抽出
            var targetCodeData = getTargetCodeData(ret.Code);
            //銘柄コードのデータが無ければ終了
            if (targetCodeData == null)
            {
                return ret;
            }
            ret.Eps = targetCodeData.Eps;
            ret.Per = targetCodeData.Per;
            ret.Capitalization = targetCodeData.Capitalization;

            //ターゲットの銘柄コードの価格リストだけを抽出
            var targetCodePriceList = geTargetCodetPriceList(ret.Code);
            //銘柄コードのデータが無ければ終了
            if (targetCodePriceList == null || targetCodePriceList.Count() == 0)
            {
                return ret;
            }

            //ターゲットの名前取得
            ret.Name = targetCodePriceList.First().Name;

            {
                TimeSpan ts1 = new TimeSpan(1, 0, 0, 0);
                TimeSpan tsOffset = new TimeSpan(offset, 0, 0, 0);

                ret.ReferenceDayKabuka = getTargetDateData(targetCodePriceList, referenceDate);
                ret.OffsetDaysBeforeKabuka = getTargetDateData(targetCodePriceList, ret.ReferenceDayKabuka.Date - tsOffset);

                ret.NextDayKabuka = getTargetNextDateData(targetCodePriceList, ret.ReferenceDayKabuka.Date);
                ret.OffsetDaysAfterKabuka = getTargetNextDateData(targetCodePriceList, ret.ReferenceDayKabuka.Date + tsOffset);


                ret.OffsetDaysAfterHighLow = getHighLowPrice(targetCodePriceList, ret.ReferenceDayKabuka.Date+ts1, ret.ReferenceDayKabuka.Date + tsOffset);
                ret.OffsetDaysBeforeHighLow = getHighLowPrice(targetCodePriceList, ret.ReferenceDayKabuka.Date - tsOffset, ret.ReferenceDayKabuka.Date );

                ret.LatestKabuka = getKabukaElementAt(targetCodePriceList, 0);
                ret.Latest5SMA = ((getKabukaElementAt(targetCodePriceList, 0).Price 
                    + getKabukaElementAt(targetCodePriceList, 1).Price 
                    + getKabukaElementAt(targetCodePriceList, 2).Price
                    + getKabukaElementAt(targetCodePriceList, 3).Price
                    + getKabukaElementAt(targetCodePriceList, 4).Price) / 5);
                ret.Yesterday5SMA = ((getKabukaElementAt(targetCodePriceList, 1).Price 
                    + getKabukaElementAt(targetCodePriceList, 2).Price 
                    + getKabukaElementAt(targetCodePriceList, 3).Price
                    + getKabukaElementAt(targetCodePriceList, 4).Price
                    + getKabukaElementAt(targetCodePriceList, 5).Price) / 5);

                ret.Latest5SMACustom = ((getKabukaElementAt(targetCodePriceList, 0).Price * 3
                    + getKabukaElementAt(targetCodePriceList, 1).Price * 2
                    + getKabukaElementAt(targetCodePriceList, 2).Price
                    + getKabukaElementAt(targetCodePriceList, 3).Price
                    + getKabukaElementAt(targetCodePriceList, 4).Price) / 8);
                ret.Yesterday5SMACustom = ((getKabukaElementAt(targetCodePriceList, 1).Price * 3
                    + getKabukaElementAt(targetCodePriceList, 2).Price * 2
                    + getKabukaElementAt(targetCodePriceList, 3).Price
                    + getKabukaElementAt(targetCodePriceList, 4).Price
                    + getKabukaElementAt(targetCodePriceList, 5).Price) / 8);

            }

            return ret;
        }



        public string OutputTsv(OutputKabuka kabukaRecode)
        {
            var k = kabukaRecode;
            //名前
            string name = k.Name;

            //時価総額
            string cap = (k.Capitalization/100.0).ToString("0.00");

            //基準日
            string rd = "'"+k.ReferenceDayKabuka.Date.ToString("MM/dd");
            string rp = k.ReferenceDayKabuka.Price.ToString();

            //基準日の次の日
            string nd = "'" + k.NextDayKabuka.Date.ToString("MM/dd");
            string nhp = k.NextDayKabuka.HighPrice.ToString();
            string nhpp = "";
            if (k.NextDayKabuka.Price != null && k.ReferenceDayKabuka.Price != null)
            {
                nhpp = ((k.NextDayKabuka.HighPrice - k.ReferenceDayKabuka.Price) / k.ReferenceDayKabuka.Price)?.ToString("p2");
            }



            //オフセット(入力)日後
            string oad = k.OffsetDaysAfterKabuka.Date.ToString("yyyy/MM/dd");
            string oap = k.OffsetDaysAfterKabuka.Price.ToString();
            string oapp = "";
            if (k.ReferenceDayKabuka.Price != null && k.OffsetDaysAfterKabuka.Price != null)
            {
                oapp = ((k.OffsetDaysAfterKabuka.Price - k.ReferenceDayKabuka.Price) / k.ReferenceDayKabuka.Price)?.ToString("p2");
            }

            //基準日からオフセット(入力)日後までの最大
            string oahd = "'" + k.OffsetDaysAfterHighLow.HighestDate?.ToString("MM/dd");
            string oahp = k.OffsetDaysAfterHighLow.HighestPrice.ToString();
            string oahpp = "";
            if (k.ReferenceDayKabuka.Price != null && k.OffsetDaysAfterHighLow.HighestPrice != null)
            {
                oahpp = ((k.OffsetDaysAfterHighLow.HighestPrice - k.ReferenceDayKabuka.Price) / k.ReferenceDayKabuka.Price)?.ToString("p2");
            }

            //基準日からオフセット(入力)日後までの最小
            string oald = "'" + k.OffsetDaysAfterHighLow.LowestDate?.ToString("MM/dd");
            string oalp = k.OffsetDaysAfterHighLow.LowestPrice.ToString();
            string oalpp = "";
            if (k.ReferenceDayKabuka.Price != null && k.OffsetDaysAfterHighLow.LowestPrice != null)
            {
                oalpp = ((k.OffsetDaysAfterHighLow.LowestPrice - k.ReferenceDayKabuka.Price) / k.ReferenceDayKabuka.Price)?.ToString("p2");
            }

            //最新株価
            string ld = "'" + k.LatestKabuka.Date.ToString("MM/dd"); ;
            string lp = k.LatestKabuka.Price.ToString();
            string lpp = ((k.LatestKabuka.Price - k.ReferenceDayKabuka.Price) / k.ReferenceDayKabuka.Price)?.ToString("p2");

            //5日移動平均の変化率
            string sma5 = ((k.Latest5SMA - k.Yesterday5SMA) / k.Yesterday5SMA)?.ToString("p2");

            //加重5日移動平均の変化率
            string sma5cu = ((k.Latest5SMACustom - k.Yesterday5SMACustom) / k.Yesterday5SMACustom)?.ToString("p2");


            //基準日を起点にした動き
            string sb = createSparkLineBefore(k.ReferenceDayKabuka,k.OffsetDaysBeforeHighLow);
            string sa = createSparkLineAfter(k.ReferenceDayKabuka,k.OffsetDaysAfterHighLow,k.NextDayKabuka,k.LatestKabuka);

            //予想PER
            string per = k.Per.ToString("0.00");

            string per15p = (k.Eps * 15).ToString("0");
            string per40p = (k.Eps * 40).ToString("0");

            //楽観per
            string rPer = (k.LatestKabuka.Price / (k.QuarterEps*4))?.ToString("0.00");

            string rPer15p = ((k.QuarterEps * 4) * 15).ToString("0");
            string rPer40p = ((k.QuarterEps * 4) * 40).ToString("0");

            string[] ret = { name, cap, ld, lp , sma5, sma5cu, lpp, rd, rp, nd, nhp, nhpp, oahd, oahp, oahpp, oald, oalp, oalpp, sa, per, per15p, per40p, rPer, rPer15p, rPer40p };
            return string.Join("\t", ret);
        }

        private IEnumerable<StockPriceCSVRecode> geTargetCodetPriceList(string targetCode)
        {

            return from p in srcPriceList
                   where p.Code == targetCode
                   select p;
        }

        private StockDataCSVRecode getTargetCodeData(string targetCode) {
            return (from p in srcDataList
                    where p.Code == targetCode
                    select p).FirstOrDefault();
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

        private Kabuka getKabukaElementAt(IEnumerable<StockPriceCSVRecode> targetList,int at) {
            var targetDateKabuka = (from p in targetList
                                    orderby p.PriceDate descending
                                    select p).ElementAt(at);

            if (targetDateKabuka != null)
            {
                return new Kabuka(targetDateKabuka.PriceDate, targetDateKabuka.EndPrice, targetDateKabuka.HighPrice, targetDateKabuka.HighPrice);
            }
            else
            {
                return null;
            }
        }





        private Kabuka getTargetDateData(IEnumerable<StockPriceCSVRecode> targetList,DateTime targetDate)
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
        private Kabuka getTargetNextDateData(IEnumerable<StockPriceCSVRecode> targetList, DateTime targetDate)
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

        private KabukaHighLow getHighLowPrice(IEnumerable<StockPriceCSVRecode> targetList, DateTime start,DateTime end)
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
