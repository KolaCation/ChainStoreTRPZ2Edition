using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using ChainStore.DataAccessLayer.Identity;
using ChainStoreTRPZ2Edition.Messages;
using ChainStoreTRPZ2Edition.ViewModels.Account;
using ChainStoreTRPZ2Edition.ViewModels.ClientOperations;
using ChainStoreTRPZ2Edition.ViewModels.Stores;
using DevExpress.Mvvm;

namespace ChainStoreTRPZ2Edition.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Properties

        private readonly IAuthenticator _authenticator;
        public List<ViewModelBase> ViewModels { get; }

        private ViewModelBase _currentViewModel;
        private int _loginStatus;
        private string _username;

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

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
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
        public MainViewModel(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
            OpenLoginControl = new RelayCommand(() => { CurrentViewModel = GetAppropriateViewModel(nameof(LoginViewModel)); });
            OpenRegisterControl = new RelayCommand(() => { CurrentViewModel = GetAppropriateViewModel(nameof(RegisterViewModel)); });
            Messenger.Default.Register<NavigationMessage>(this, HandleNavigation);
            Messenger.Default.Register<LoginMessage>(this, HandleMenuIcon);
        }

        /// <summary>
        /// Constructor for injecting ViewModels
        /// </summary>
        /// <param name="authenticator">For providing current user's email in sidebar menu</param>
        /// <param name="registerViewModel"></param>
        /// <param name="loginViewModel"></param>
        /// <param name="storeViewModel"></param>
        /// <param name="storeDetailsViewModel"></param>
        /// <param name="purchaseViewModel"></param>
        /// <param name="bookViewModel"></param>
        public MainViewModel(IAuthenticator authenticator, RegisterViewModel registerViewModel, LoginViewModel loginViewModel, StoresViewModel storeViewModel,
            StoreDetailsViewModel storeDetailsViewModel, PurchaseViewModel purchaseViewModel, BookViewModel bookViewModel) : this(authenticator)
        {
            Username = "Unauthorized";
            ViewModels = new List<ViewModelBase> { registerViewModel, loginViewModel, storeViewModel, storeDetailsViewModel, purchaseViewModel, bookViewModel };
            CurrentViewModel = GetAppropriateViewModel(nameof(LoginViewModel));
        }


        #endregion

        #region Methods

        private ViewModelBase GetAppropriateViewModel(string viewModelName)
        {
            return ViewModels.Find(e => e.GetType().Name.Equals(viewModelName));
        }

        #endregion

        #region Handlers

        private void HandleNavigation(NavigationMessage navigationMessage)
        {
            Messenger.Default.Send(new RefreshDataMessage(navigationMessage.ViewModelName, navigationMessage.ItemId));
            CurrentViewModel = GetAppropriateViewModel(navigationMessage.ViewModelName);
        }
        
        private void HandleMenuIcon(LoginMessage loginMessage)
        {
            if (loginMessage.IsLoggedIn)
            {
                Username = _authenticator.GetCurrentUser().UserName;
                LoginStatus = 2;
            }
            else
            {
                Username = "Unauthorized";
            }

        }

        #endregion
    }
}