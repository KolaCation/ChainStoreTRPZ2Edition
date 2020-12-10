using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChainStore.DataAccessLayer.Identity;
using ChainStoreTRPZ2Edition.DataInterfaces;
using ChainStoreTRPZ2Edition.Messages;
using ChainStoreTRPZ2Edition.ViewModels.Stores;
using DevExpress.Mvvm;

namespace ChainStoreTRPZ2Edition.ViewModels.Account
{
    public class LoginViewModel : ViewModelBase, ICleanable
    {
        #region Properties

        private readonly IAuthenticator _authenticator;
        public string Email
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string ErrorMessage
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        #endregion

        #region Commands

        public ICommand NavigateToRegister { get; set; }

        public ICommand Login { get; set; }

        #endregion

        #region Constructor
        public LoginViewModel(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
            Login = new RelayCommand(async passwordBox => await HandleLogin(passwordBox));
            NavigateToRegister = new RelayCommand(() =>
            {
                ClearData();
                Messenger.Default.Send(new NavigationMessage(nameof(RegisterViewModel)));
            });
        }

        #endregion

        #region Methods
        public void ClearData()
        {
            Email = string.Empty;
            ErrorMessage = string.Empty;
        }

        #endregion

        #region Handlers
        private async Task HandleLogin(object passwordBox)
        {
            if (string.IsNullOrEmpty(((PasswordBox)passwordBox).Password) || string.IsNullOrEmpty(Email))
            {
                ErrorMessage = "Provide email and password to log in.";
            }
            else
            {
                var tryLogin = await _authenticator.Login(Email, ((PasswordBox)passwordBox).Password);
                if (tryLogin)
                {
                    ClearData();
                    Messenger.Default.Send(new NavigationMessage(nameof(StoresViewModel)));
                    Messenger.Default.Send(new LoginMessage(true));
                }
                else
                {
                    ErrorMessage = "Invalid credentials.";
                }
            }
        }
        
        #endregion
    }
}