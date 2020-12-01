using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChainStore.DataAccessLayer.Identity;
using ChainStoreTRPZ2Edition.Helpers;
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
                MessageBox.Show("TRY REGISTER!");
                var unpackedPasswordInputBoxes = (object[]) passwordInputBoxes;
                var validationResult = HandleValidation(unpackedPasswordInputBoxes);
                if (validationResult.IsValid)
                {
                    var tryRegister = await _authenticator.Register(Name, Email, ((PasswordBox)unpackedPasswordInputBoxes[0]).Password,
                        ((PasswordBox)unpackedPasswordInputBoxes[1]).Password);
                    if (tryRegister)
                    {
                        NavigateToSignIn.Execute(null);
                    }
                    else
                    {
                        MessageBox.Show("Something went wrong. Try to register later.");
                    }
                }
                else
                {
                    MessageBox.Show(validationResult.ErrorMessage);
                }
            });
            NavigateToSignIn = new RelayCommand(() => Messenger.Default.Send(new NavigationMessage(nameof(LoginViewModel))));
        }


        private CustomValidationResult HandleValidation(object[] passwords)
        {
            var result = new CustomValidationResult(null, true);
            if (string.IsNullOrEmpty(Name) || Name.Length < 2 || Name.Length > 60)
            {
                result = new CustomValidationResult("Provide valid name.", false);
            }
            try
            {
                var email = new MailAddress(Email);
            }
            catch (Exception)
            {
                if (result.IsValid)
                {
                    result = new CustomValidationResult("Provide valid email.", false);
                }
                else
                {
                    result.AppendErrorMessageContent("Provide valid email.");
                }
            }

            if (passwords == null || ((PasswordBox) passwords[0]).Password != ((PasswordBox) passwords[1]).Password)
            {
                if (result.IsValid)
                {
                    result = new CustomValidationResult("Passwords do not match.", false);
                }
                else
                {
                    result.AppendErrorMessageContent("Passwords do not match.");
                }
            }

            return result;
        }
    }
}