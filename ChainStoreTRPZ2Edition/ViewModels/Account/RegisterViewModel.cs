using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChainStoreTRPZ2Edition.ViewModels.Account
{
    public sealed class RegisterViewModel : BaseViewModel
    {
        private string _name;
        private string _email;
        private SecureString _password;
        private SecureString _confirmPassword;

        #region Properties

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public SecureString Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public SecureString ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }

        #endregion

        public ICommand ShowMessageBox { get; set; }
        public RegisterViewModel()
        {
            ShowMessageBox = new RelayCommand((passwordInputBoxes) =>
            {
                var passwords = (object[]) passwordInputBoxes;
                MessageBox.Show($"{_name} | {_email} | {(passwords[0] as PasswordBox).Password} | {(passwords[1] as PasswordBox).Password}");
            });
        }
    }
}