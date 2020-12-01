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

        #region Properties
        public string Email { get; set; }
        public string Password { get; set; }

        #endregion

        #region Commands
        public ICommand NavigateToSignUp { get; set; }

        public ICommand Login { get; set; }

        #endregion

        public LoginViewModel(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
            Login = new RelayCommand(async (passwordBox) =>
            {
                MessageBox.Show("TRY LOGIN!");
                if (string.IsNullOrEmpty(((PasswordBox) passwordBox).Password) || string.IsNullOrEmpty(Email))
                {
                    MessageBox.Show("Provide email and password to login.");
                }
                else
                {
                    var tryLogin = await _authenticator.Login(Email, ((PasswordBox) passwordBox).Password);
                    if (tryLogin)
                    {
                        Messenger.Default.Send(new NavigationMessage(nameof(StoreViewModel)));
                    }
                    else
                    {
                        MessageBox.Show("Invalid Credentials.");
                    }
                }
            });
            NavigateToSignUp = new RelayCommand(()=>Messenger.Default.Send(new NavigationMessage(nameof(RegisterViewModel))));
        }
    }
}
