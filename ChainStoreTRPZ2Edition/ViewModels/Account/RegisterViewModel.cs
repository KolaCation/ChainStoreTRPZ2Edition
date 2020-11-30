using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChainStore.DataAccessLayer.Identity;
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

        public ICommand ShowMessageBox { get; set; }
        public RegisterViewModel(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
            ShowMessageBox = new RelayCommand((passwordInputBoxes) =>
            {
                var passwords = (object[]) passwordInputBoxes;
                MessageBox.Show($"{Name} | {Email} | {(passwords[0] as PasswordBox).Password} | {(passwords[1] as PasswordBox).Password}");
                Messenger.Default.Send("From RegisterViewModel To MainViewModel: ShowMessageBox done!");
            });
        }
    }
}