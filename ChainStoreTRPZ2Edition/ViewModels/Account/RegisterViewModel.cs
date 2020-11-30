using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChainStore.DataAccessLayer.Identity;
using ChainStoreTRPZ2Edition.Messages;
using DevExpress.Mvvm;

namespace ChainStoreTRPZ2Edition.ViewModels.Account
{
    public sealed class RegisterViewModel : ViewModelBase
    {
        private readonly IAuthenticator _authenticator;

        #region Properties

        public string Name { get; set; }

        public string Email { get; set; }

        #endregion

        #region Commands

        public ICommand NavigateToSignIn { get; set; }
        public ICommand ShowMessageBox { get; set; }

        #endregion
        public RegisterViewModel(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
            ShowMessageBox = new RelayCommand((passwordInputBoxes) =>
            {
                var passwords = (object[]) passwordInputBoxes;
                MessageBox.Show($"{Name} | {Email} | {(passwords[0] as PasswordBox).Password} | {(passwords[1] as PasswordBox).Password}");
                Messenger.Default.Send(new NavigationMessage(nameof(LoginViewModel)));
            });
            NavigateToSignIn = new RelayCommand(() => Messenger.Default.Send(new NavigationMessage(nameof(LoginViewModel))));
        }
    }
}