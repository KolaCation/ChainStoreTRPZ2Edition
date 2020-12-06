using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChainStore.DataAccessLayer.Identity;
using ChainStoreTRPZ2Edition.DataInterfaces;
using ChainStoreTRPZ2Edition.Helpers;
using ChainStoreTRPZ2Edition.Messages;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;

namespace ChainStoreTRPZ2Edition.ViewModels.Account
{
    public sealed class RegisterViewModel : ViewModelBase, ICleanable
    {
        #region Properties

        private readonly IAuthenticator _authenticator;
        public string Name
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

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

        public ICommand NavigateToSignIn { get; set; }
        public ICommand Register { get; set; }

        #endregion

        #region Constructor
        public RegisterViewModel(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
            Register = new RelayCommand(async passwordInputBoxes => await HandleRegistration(passwordInputBoxes));
            NavigateToSignIn = new RelayCommand(() =>
            {
                ClearData();
                Messenger.Default.Send(new NavigationMessage(nameof(LoginViewModel)));
            });
        }

        #endregion

        #region Methods
        public void ClearData()
        {
            Name = string.Empty;
            Email = string.Empty;
        }

        #endregion

        #region Handlers
        private async Task HandleRegistration(object passwordInputBoxes)
        {
            var unpackedPasswordInputBoxes = (object[])passwordInputBoxes;
            var validationResult = HandleValidation(unpackedPasswordInputBoxes);
            if (validationResult.IsValid)
            {
                var tryRegister = await _authenticator.Register(Name, Email,
                    ((PasswordBox)unpackedPasswordInputBoxes[0]).Password,
                    ((PasswordBox)unpackedPasswordInputBoxes[1]).Password);
                if (tryRegister == RegistrationResult.Success)
                {
                    ClearData();
                    NavigateToSignIn.Execute(null);
                }
                else if (tryRegister == RegistrationResult.EmailAlreadyTaken)
                {
                    ErrorMessage = "Email is already taken.";
                }
                else
                {
                    ErrorMessage = "Something went wrong. Try to register later.";
                }
            }
            else
            {
                ErrorMessage = validationResult.ErrorMessage;
            }
        }
        private CustomValidationResult HandleValidation(object[] passwords)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");
            var result = new CustomValidationResult(null, true);
            if (string.IsNullOrEmpty(Name) || Name.Length < 2 || Name.Length > 60)
            {
                result = new CustomValidationResult("Provide valid name.", false);
                return result;
            }

            try
            {
                var email = new MailAddress(Email);
            }
            catch (Exception)
            {
                result = new CustomValidationResult("Provide valid email.", false);
                return result;
            }

            if (passwords == null || !hasMinimum8Chars.IsMatch(((PasswordBox)passwords[0]).Password))
            {
                result = new CustomValidationResult("Password must contain at least 6 characters.", false);
                return result;
            }
            else if (!hasUpperChar.IsMatch(((PasswordBox)passwords[0]).Password))
            {
                result = new CustomValidationResult("Password must contain at least 1 upper.", false);
                return result;
            }
            else if (!hasNumber.IsMatch(((PasswordBox)passwords[0]).Password))
            {
                result = new CustomValidationResult("Password must contain at least 1 number.", false);
                return result;
            }
            else if (((PasswordBox)passwords[0]).Password != ((PasswordBox)passwords[1]).Password)
            {
                result = new CustomValidationResult("Passwords do not match.", false);
                return result;
            }

            return result;
        }

        #endregion
    }
}