using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using ChainStore.DataAccessLayer.Identity;
using ChainStoreTRPZ2Edition.Admin.ViewModels;
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
        private string _menuIconVisibility;
        private int _menuIconZIndex;
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

        public string MenuIconVisibility
        {
            get => _menuIconVisibility;
            set
            {
                _menuIconVisibility = value;
                SetValue(value);
            }
        }

        public int MenuIconZIndex
        {
            get => _menuIconZIndex;
            set
            {
                _menuIconZIndex = value;
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

        public ICommand NavigateToLogin { get; set; }
        public ICommand NavigateToProfile { get; set; }
        public ICommand NavigateToStoresIndex { get; set; }
        public ICommand Logout { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for creating commands, handling messages
        /// </summary>
        public MainViewModel(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
            NavigateToLogin = new RelayCommand(() =>
            {
                CurrentViewModel = GetAppropriateViewModel(nameof(LoginViewModel));
            });
            NavigateToProfile = new RelayCommand(() =>
            {
                if (_authenticator.IsLoggedIn())
                {
                    HandleNavigation(new NavigationMessage(nameof(ProfileViewModel),
                        _authenticator.GetCurrentUser().ClientId));
                    
                }
                else
                {
                    MessageBox.Show("Login to access profile.");
                }
            });
            NavigateToStoresIndex = new RelayCommand(() =>
            {
                if (_authenticator.IsLoggedIn())
                {
                    HandleNavigation(new NavigationMessage(nameof(StoresViewModel)));
                }
                else
                {
                    MessageBox.Show("Login to access stores.");
                }
            });
            Logout = new RelayCommand(() =>
            {
                _authenticator.Logout();
                HandleMenuIcon(new LoginMessage(false));
                NavigateToLogin.Execute(null);
            });
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
        /// <param name="profileViewModel"></param>
        /// <param name="categoriesViewModel"></param>
        public MainViewModel(IAuthenticator authenticator, RegisterViewModel registerViewModel,
            LoginViewModel loginViewModel, StoresViewModel storeViewModel,
            StoreDetailsViewModel storeDetailsViewModel, PurchaseViewModel purchaseViewModel,
            BookViewModel bookViewModel, ProfileViewModel profileViewModel,
            CategoriesViewModel categoriesViewModel) : this(authenticator)
        {
            Username = "Unauthorized";
            MenuIconVisibility = "Collapsed";
            ViewModels = new List<ViewModelBase>
            {
                registerViewModel, loginViewModel, storeViewModel, storeDetailsViewModel, purchaseViewModel,
                bookViewModel, profileViewModel, categoriesViewModel
            };
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
                MenuIconVisibility = "Visible";
                MenuIconZIndex = 2;
            }
            else
            {
                Username = "Unauthorized";
                MenuIconVisibility = "Collapsed";
                MenuIconZIndex = 0;
            }
        }

        #endregion
    }
}