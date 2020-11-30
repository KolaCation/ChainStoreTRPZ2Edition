using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChainStore.DataAccessLayer.Identity;
using ChainStoreTRPZ2Edition.Messages;
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

        public ICommand ShowMessageBox { get; set; }

        #endregion

        public LoginViewModel(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
            ShowMessageBox = new RelayCommand(async (passwordBox) =>
            {
                MessageBox.Show("TRY LOGIN!");
                var res = await _authenticator.Login(Email, (passwordBox as PasswordBox).Password);
                MessageBox.Show(res.ToString());
            });
            NavigateToSignUp = new RelayCommand(()=>Messenger.Default.Send(new NavigationMessage(nameof(RegisterViewModel))));
        }
    }
}
