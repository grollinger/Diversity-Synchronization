using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using GalaSoft.MvvmLight.Threading;


namespace MVVMDiversity.ViewModel
{
    public class DelegateCommand : DelegateCommand<object>
    {
        public DelegateCommand(Action execute)
            : this(execute, null)
        {

        }

        public DelegateCommand(Action execute, Func<bool> canExecute)
            : base(
                (param)=>
                { 
                    if(execute != null)
                        execute();
                },
                (param) =>
                {
                    if(canExecute != null)
                        return canExecute();
                    else
                        return true;
                })
        {

        }
    }

    public class DelegateCommand<T> : ICommand
    {
        private readonly Predicate<T> _canExecute;
        private readonly Action<T> _execute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        public DelegateCommand(Action<T> execute,
                       Predicate<T> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute((parameter is T)? (T) parameter:default(T));
        }

        public void Execute(object parameter)
        {
            _execute((parameter is T) ? (T)parameter : default(T));
        }

        public void RaiseCanExecuteChanged()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    if (CanExecuteChanged != null)
                    {
                        CanExecuteChanged(this, EventArgs.Empty);
                    }
                });
        }
    }
}
