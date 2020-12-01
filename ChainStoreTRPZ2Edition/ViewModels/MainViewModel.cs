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
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public List<ViewModelBase> ViewModels { get; set; }

        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

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
        }

        /// <summary>
        /// Constructor for injecting ViewModels
        /// </summary>
        /// <param name="registerViewModel"></param>
        /// <param name="loginViewModel"></param>
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

        #endregion

        #region INotifyPropertyChanged Realisation

        public new event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}