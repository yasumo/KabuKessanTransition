using CsvHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private List<CsvKabukaRecode> csvData;
        #endregion

        public MainWindowViewModel() {
            CsvDir = "E:\\data\\dropbox\\Dropbox\\program\\kabu\\sh\\data";
            offsetDays = 100;
            csvData = new List<CsvKabukaRecode>();
            loadCSVCommandExecute();
            PropertyChanged += propertyChange;
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

        private void propertyChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("CodeAndDate") )
            {
                outputCommandExecute();
            }
        }
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
                    using (CsvParser parser = new CsvParser(new StreamReader(csvFilePath, Encoding.GetEncoding("Shift-JIS"))))
                    {
                        parser.Configuration.HasHeaderRecord = true;
                        parser.Configuration.RegisterClassMap<KabukaMap>();

                        using (CsvReader reader = new CsvReader(parser))
                        {
                            csvData.AddRange(reader.GetRecords<CsvKabukaRecode>().ToList());
                        }
                    }
                }
            }



        }


        private void outputCommandExecute()
        {
            OutputTsv = "";
            using (CsvParser parser = new CsvParser(new StringReader(CodeAndDate)))
            {
                parser.Configuration.HasHeaderRecord = false;
                parser.Configuration.Delimiter = "\t";
                parser.Configuration.RegisterClassMap<InputCodeMap>();

                using (CsvReader reader = new CsvReader(parser))
                {
                    try
                    {
                        var inputCodeAndDateList = reader.GetRecords<CsvInputCodeRecode>().ToList();

                        var outputService = new OutputDataService(csvData);
                        foreach (var codeAndDate in inputCodeAndDateList)
                        {
                            var kabuka = outputService.SearchKabuka(codeAndDate.Code, codeAndDate.Date, codeAndDate.Offset);
                            var tsvRecode = outputService.OutputTsv(kabuka);
                            OutputTsv += tsvRecode + "\n";
                        }
                    }
                    catch {
                        //入力値がパース出来なかった時用、もうちょっと良いやり方が良い
                        return;
                    }

                }
            }
            if (OutputTsv != null) { 
                Clipboard.SetText(OutputTsv);
            }

        }



    }
}
