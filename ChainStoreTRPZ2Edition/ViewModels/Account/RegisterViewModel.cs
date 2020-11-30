using System;
using System.Collections.Generic;
using System.Net.Mail;
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
            ShowMessageBox = new RelayCommand(async passwordInputBoxes =>
            {
                var passwords = (object[]) passwordInputBoxes;
                var validationResult = HandleValidation(passwords);
                if (validationResult)
                {
                    MessageBox.Show("SUCCESS!");
                    var res = await _authenticator.Register(Name, Email, (passwords[0] as PasswordBox).Password,
                        (passwords[1] as PasswordBox).Password);
                    MessageBox.Show(res.ToString());
                }
                else
                {
                    MessageBox.Show("FAIL!");
                }
                //MessageBox.Show($"COMMAND {Name} | {Email} | {(passwords[0] as PasswordBox).Password} | {(passwords[1] as PasswordBox).Password}");
            });
            NavigateToSignIn = new RelayCommand(() => Messenger.Default.Send(new NavigationMessage(nameof(LoginViewModel))));
        }


        private bool HandleValidation(object[] passwords)
        {
            if (string.IsNullOrEmpty(Name) || Name.Length < 2 || Name.Length > 60) return false;
            try
            {
                var email = new MailAddress(Email);
            }
            catch (Exception)
            {
                return false;
            }

            if (passwords == null || (passwords[0] as PasswordBox).Password != (passwords[1] as PasswordBox).Password) return false;
            return true;
        }
    }
}