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
        private Action<object> _actionWithParam;
        private Func<object, bool> _canExecute;

        #endregion

        #region Public events

        public event EventHandler CanExecuteChanged = (sender, e) => { };

        #endregion

        #region Conctructor

        public RelayCommand(Action action, Func<object, bool> canExecute = null)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public RelayCommand(Action<object> actionWithParam, Func<object, bool> canExecute = null)
        {
            _actionWithParam = actionWithParam;
            _canExecute = canExecute;
        }

        #endregion

        #region Command methods

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            if (_action == null)
            {
                _actionWithParam(parameter);
            }
            else
            {
                _action();
            }
        }

        #endregion
    }
}