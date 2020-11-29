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
        private string _name;
        private string _email;

        #region Properties

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
            }
        }

        #endregion

        public ICommand ShowMessageBox { get; set; }
        public RegisterViewModel(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
            ShowMessageBox = new RelayCommand((passwordInputBoxes) =>
            {
                var passwords = (object[]) passwordInputBoxes;
                MessageBox.Show($"{_name} | {_email} | {(passwords[0] as PasswordBox).Password} | {(passwords[1] as PasswordBox).Password}");
                Messenger.Default.Send("From RegisterViewModel To MainViewModel: ShowMessageBox done!");
            });
        }
    }
}