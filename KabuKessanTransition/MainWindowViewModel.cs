using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private List<Kabuka> csvData;
        #endregion

        public MainWindowViewModel() {
            CsvDir = "E:\\data\\dropbox\\Dropbox\\program\\kabu\\sh\\data";
            offsetDays = 20;
            csvData = new List<Kabuka>();
        }

        #region コマンド
        private DelegateCommand loadCSVCommand;

        public DelegateCommand LoadCSVCommand
        {
            get { return this.loadCSVCommand ?? (this.loadCSVCommand = new DelegateCommand(loadCSVCommandExecute, null)); }
        }


        private DelegateCommand outputCommand;

        public DelegateCommand OutputCommand
        {
            get { return this.outputCommand ?? (this.outputCommand = new DelegateCommand(outputCommandExecute, null)); }
        }

        #endregion
        private void loadCSVCommandExecute()
        {

            //今日の日付
            var today = DateTime.Today;

            for (var i = 0; i < OffsetDays; i++) {
                TimeSpan ts = new TimeSpan(i, 0, 0, 0);
                var tmpDay = today - ts;
                var csvName = "\\\\japan-all-stock-prices_"+ tmpDay.ToString("yyyyMMdd")+".csv";
                var csvFilePath = CsvDir + csvName;
                if (File.Exists(csvFilePath))
                {
                    CsvParser parser = new CsvParser(new StreamReader(csvFilePath, Encoding.GetEncoding("Shift-JIS")));
                    parser.Configuration.HasHeaderRecord = true; // ヘッダ行は無い
                    parser.Configuration.RegisterClassMap<KabukaMap>();

                    CsvReader reader = new CsvReader(parser);
                    csvData.AddRange(reader.GetRecords<Kabuka>().ToList());
                }
            }

            var ho = from p in csvData
                     where p.Code == "1333"
                     select p;

            foreach (var hoge in ho) {
                Console.WriteLine(hoge.Code);
                Console.WriteLine(hoge.Name);
                Console.WriteLine(hoge.EndPrice);
                Console.WriteLine(hoge.HighPrice);
            }

        }


        private void outputCommandExecute()
        {
            Console.WriteLine(csvDir);
            Console.WriteLine(codeAndDate);
            Console.WriteLine(outputTsv);
//            var hogehoge = DateTime.ParseExact("2004/11/24 20:23", "yyyy/M/dd HH:mm", null);
//            var hoge =  DateTime.ParseExact("2017/3/17 15:15", "yyyy/MM/dd HH:mm", null);
        }



    }
}
