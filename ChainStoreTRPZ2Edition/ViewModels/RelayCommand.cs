using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ChainStoreTRPZ2Edition.ViewModels
{
    public sealed class RelayCommand : ICommand
    {
        #region Private Members
        private Action _action;
        #endregion

        #region Public events
        public event EventHandler CanExecuteChanged = (sender, e) => { };
        #endregion

        #region Conctructor
        public RelayCommand(Action action)
        {
            _action = action;
        }
        #endregion

        #region Command methods
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action();
        }
        #endregion
    }
}

