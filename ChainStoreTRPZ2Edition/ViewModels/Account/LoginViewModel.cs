using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChainStore.DataAccessLayer.Identity;
using ChainStoreTRPZ2Edition.Messages;
using ChainStoreTRPZ2Edition.ViewModels.Stores;
using DevExpress.Mvvm;

namespace ChainStoreTRPZ2Edition.ViewModels.Account
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthenticator _authenticator;
        private string _email;
        private string _errorMessage;

        #region Properties

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                SetValue(value);
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                SetValue(value);
            }
        }

        #endregion

        #region Commands

        public ICommand NavigateToSignUp { get; set; }

        public ICommand Login { get; set; }

        #endregion

        public LoginViewModel(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
            Login = new RelayCommand(async passwordBox =>
            {
                if (string.IsNullOrEmpty(((PasswordBox) passwordBox).Password) || string.IsNullOrEmpty(Email))
                {
                    ErrorMessage = "Provide email and password to log in.";
                }
                else
                {
                    var tryLogin = await _authenticator.Login(Email, ((PasswordBox) passwordBox).Password);
                    if (tryLogin)
                    {
                        ErrorMessage = string.Empty;
                        Email = string.Empty;
                        Messenger.Default.Send(new NavigationMessage(nameof(StoreViewModel)));
                        Messenger.Default.Send(new LoginMessage(true));
                    }
                    else
                    {
                        ErrorMessage = "Invalid credentials.";
                    }
                }
            });
            NavigateToSignUp = new RelayCommand(() =>
            {
                ErrorMessage = string.Empty;
                Email = string.Empty;
                Messenger.Default.Send(new NavigationMessage(nameof(RegisterViewModel)));
            });
        }
    }
}