using ChainStore.DataAccessLayer.Identity;

namespace ChainStoreTRPZ2Edition.ViewModels.Account
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthenticator _authenticator;
        private string _email;
        private string _password;

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public LoginViewModel(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
        }
    }
}
