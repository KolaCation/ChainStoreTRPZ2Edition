using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChainStore.DataAccessLayer.Identity;
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


        public ICommand ShowMessageBox { get; set; }
        public LoginViewModel(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
            ShowMessageBox = new RelayCommand(() =>
            {
                MessageBox.Show("Hello from loginVM!");
            });
        }
    }
}
