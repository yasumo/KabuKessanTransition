using CsvHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KabuKessanTransition
{
    class MainWindowViewModel : BindableBase
    {

        #region プロパティ・プライベート変数
        private string csvDir;
        public string CsvDir
        {
            get { return csvDir; }
            set { this.SetProperty(ref this.csvDir, value); }
        }
        private string codeAndDate;
        public string CodeAndDate
        {
            get { return codeAndDate; }
            set { this.SetProperty(ref this.codeAndDate, value); }
        }
        private string outputTsv;
        public string OutputTsv
        {
            get { return outputTsv; }
            set { this.SetProperty(ref this.outputTsv, value); }
        }

        private int offsetDays;
        public int OffsetDays
        {
            get { return offsetDays; }
            set { this.SetProperty(ref this.offsetDays, value); }
        }

        private List<StockPriceCSVRecode> priceCsvData;
        private List<StockDataCSVRecode> dataCsvData;

        #endregion

        public MainWindowViewModel() {
            CsvDir = "E:\\data\\dropbox\\Dropbox\\program\\kabu\\sh\\data";
            offsetDays = 100;
            priceCsvData = new List<StockPriceCSVRecode>();
            dataCsvData = new List<StockDataCSVRecode>();
            loadCSV();
            openPage();
            PropertyChanged += propertyChange;

        }

        #region コマンド
        private DelegateCommand loadCSVCommand;

        public DelegateCommand LoadCSVCommand
        {
            get { return this.loadCSVCommand ?? (this.loadCSVCommand = new DelegateCommand(loadCSVCommandExecute, null)); }
        }


        private DelegateCommand openpageCommand;

        public DelegateCommand OpenPageCommand
        {
            get { return this.openpageCommand ?? (this.openpageCommand = new DelegateCommand(openPageCommandExecute, null)); }
        }


        #endregion

        private void openPage() {
            Process.Start("C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe", "https://docs.google.com/spreadsheets/");
            Process.Start("C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe", "https://kabutan.jp/news/");
        }


        private void propertyChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("CodeAndDate") )
            {
                output();
            }
        }


        private void loadCSVCommandExecute() {
            loadCSV();
        }

        private void openPageCommandExecute()
        {
            openPage();
        }

        private void outputCommandExecute()
        {
            output();
        }


        private void loadCSV() {
            loadPriceCSV();
            loadDataCSV();
        }

        private void loadDataCSV()
        {

            //今日の日付
            var today = DateTime.Today;

            //一週間以内にデータが無ければおかしい
            for (var i = 0; i < 7; i++)
            {
                TimeSpan ts = new TimeSpan(i, 0, 0, 0);
                var tmpDay = today - ts;
                var csvName = "\\\\japan-all-stock-data_" + tmpDay.ToString("yyyyMMdd") + ".csv";
                var csvFilePath = CsvDir + csvName;
                if (File.Exists(csvFilePath))
                {
                    using (CsvParser parser = new CsvParser(new StreamReader(csvFilePath, Encoding.GetEncoding("Shift-JIS"))))
                    {
                        parser.Configuration.HasHeaderRecord = true;
                        parser.Configuration.RegisterClassMap<StockDataMap>();

                        using (CsvReader reader = new CsvReader(parser))
                        {
                            dataCsvData= reader.GetRecords<StockDataCSVRecode>().ToList();
                        }
                    }
                    return;
                }
            }


        }

        private void loadPriceCSV()
        {

            //今日の日付
            var today = DateTime.Today;

            for (var i = 0; i < OffsetDays; i++)
            {
                TimeSpan ts = new TimeSpan(i, 0, 0, 0);
                var tmpDay = today - ts;
                var csvName = "\\\\japan-all-stock-prices_" + tmpDay.ToString("yyyyMMdd") + ".csv";
                var csvFilePath = CsvDir + csvName;
                if (File.Exists(csvFilePath))
                {
                    using (CsvParser parser = new CsvParser(new StreamReader(csvFilePath, Encoding.GetEncoding("Shift-JIS"))))
                    {
                        parser.Configuration.HasHeaderRecord = true;
                        parser.Configuration.RegisterClassMap<StockPriceMap>();

                        using (CsvReader reader = new CsvReader(parser))
                        {
                            priceCsvData.AddRange(reader.GetRecords<StockPriceCSVRecode>().ToList());
                        }
                    }
                }
            }
        }


        private void output()
        {
            OutputTsv = "";
            using (CsvParser parser = new CsvParser(new StringReader(CodeAndDate)))
            {
                parser.Configuration.HasHeaderRecord = false;
                parser.Configuration.Delimiter = "\t";
                parser.Configuration.RegisterClassMap<InputCodeMap>();

                using (CsvReader reader = new CsvReader(parser))
                {
                    List<CsvInputCodeRecode> inputCodeAndDateList = null;
                    try
                    {
                        inputCodeAndDateList = reader.GetRecords<CsvInputCodeRecode>().ToList();
                    }
                    catch {
                        //入力値がパース出来なかった時用、もうちょっと良いやり方が良い
                        return;
                    }

                        var outputService = new OutputDataService(priceCsvData,dataCsvData);
                        foreach (var codeAndDate in inputCodeAndDateList)
                        {
                            var kabuka = outputService.SearchKabuka(codeAndDate.Code, codeAndDate.Date, codeAndDate.Offset,codeAndDate.QuarterEps);
                            var tsvRecode = outputService.OutputTsv(kabuka);
                            OutputTsv += tsvRecode + "\n";
                        }
                }
            }
            if (OutputTsv != null) { 
                Clipboard.SetText(OutputTsv);
            }

        }
    }
}
