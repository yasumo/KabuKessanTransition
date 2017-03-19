using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KabuKessanTransition
{
    class MainWindowViewModel : BindableBase
    {
        #region プロパティ・プライベート変数
        private string sqliteFilePath;
        public string SQLiteFilePath
        {
            get { return sqliteFilePath; }
            set { this.SetProperty(ref this.sqliteFilePath, value); }
        }
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
        #endregion

        #region コマンド
        private DelegateCommand updateDBCommand;

        public DelegateCommand UpdateDBCommand
        {
            get { return this.updateDBCommand ?? (this.updateDBCommand = new DelegateCommand(updateDBCommandExecute, null)); }
        }


        private DelegateCommand outputCommand;

        public DelegateCommand OutputCommand
        {
            get { return this.outputCommand ?? (this.outputCommand = new DelegateCommand(outputCommandExecute, null)); }
        }

        #endregion
        private void updateDBCommandExecute()
        {
            Console.WriteLine(csvDir);
            Console.WriteLine(codeAndDate);
            Console.WriteLine(outputTsv);
            Console.WriteLine(sqliteFilePath);
        }


        private void outputCommandExecute()
        {
            Console.WriteLine(csvDir);
            Console.WriteLine(codeAndDate);
            Console.WriteLine(outputTsv);
            Console.WriteLine(sqliteFilePath);

        }



    }
}
