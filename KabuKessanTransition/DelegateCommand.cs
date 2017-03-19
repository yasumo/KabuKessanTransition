
using System.Windows.Input;

namespace KabuKessanTransition
{
    public class DelegateCommand : ICommand
    {
        System.Action execute;
        System.Action<object> executeWithParam;
        System.Func<bool> canExecute;

        public bool CanExecute(object parameter)
        {
            if (canExecute == null) {
                return true;
            }
            return canExecute();
        }

        public event System.EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {

            //まともな実装にしたい
            if (execute != null)
            {
                execute();
            }
            else if(executeWithParam!=null)
            {
                executeWithParam(parameter);
            }
        }

        public DelegateCommand(System.Action execute, System.Func<bool> canExecute = null)
        {
            
            this.execute = execute;
            this.executeWithParam = null;
            this.canExecute = canExecute;
        }
        public DelegateCommand(System.Action<object> execute, System.Func<bool> canExecute = null)
        {
            this.execute = null;
            this.executeWithParam = execute;
            this.canExecute = canExecute;
        }

    }
}
