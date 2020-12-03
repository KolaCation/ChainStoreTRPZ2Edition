using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using ChainStoreTRPZ2Edition.Messages;
using ChainStoreTRPZ2Edition.ViewModels.Account;
using ChainStoreTRPZ2Edition.ViewModels.Stores;
using DevExpress.Mvvm;

namespace ChainStoreTRPZ2Edition.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Properties

        public List<ViewModelBase> ViewModels { get; }

        private ViewModelBase _currentViewModel;
        private int _loginStatus;

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                SetValue(value);
            }
        }

        public int LoginStatus
        {
            get => _loginStatus;
            set
            {
                _loginStatus = value;
                SetValue(value);
            }
        }

        #endregion

        #region Navigational Commands

        public ICommand OpenLoginControl { get; set; }
        public ICommand OpenRegisterControl { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for creating commands, handling messages
        /// </summary>
        public MainViewModel()
        {
            OpenLoginControl = new RelayCommand(() => { CurrentViewModel = GetAppropriateViewModel(nameof(LoginViewModel)); });
            OpenRegisterControl = new RelayCommand(() => { CurrentViewModel = GetAppropriateViewModel(nameof(RegisterViewModel)); });
            Messenger.Default.Register<NavigationMessage>(this, HandleNavigation);
            Messenger.Default.Register<LoginMessage>(this, HandleMenuIcon);
        }

        /// <summary>
        /// Constructor for injecting ViewModels
        /// </summary>
        /// <param name="registerViewModel"></param>
        /// <param name="loginViewModel"></param>
        /// <param name="storeViewModel"></param>
        public MainViewModel(RegisterViewModel registerViewModel, LoginViewModel loginViewModel, StoreViewModel storeViewModel) : this()
        {
            ViewModels = new List<ViewModelBase> { registerViewModel, loginViewModel, storeViewModel };
            CurrentViewModel = GetAppropriateViewModel(nameof(LoginViewModel));
        }


        #endregion

        #region Methods

        private ViewModelBase GetAppropriateViewModel(string viewModelName)
        {
            return ViewModels.Find(e => e.GetType().Name.Equals(viewModelName));
        }


        private void HandleNavigation(NavigationMessage navigationMessage)
        {
            Messenger.Default.Send(new RefreshDataMessage(navigationMessage.ViewModelName));
            CurrentViewModel = GetAppropriateViewModel(navigationMessage.ViewModelName);
        }

        private void HandleMenuIcon(LoginMessage loginMessage)
        {
            LoginStatus = loginMessage.IsLoggedIn ? 2 : 0;
        }

        #endregion
    }
}