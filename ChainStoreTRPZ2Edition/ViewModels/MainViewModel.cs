using System.Windows.Controls;
using System.Windows.Input;
using ChainStoreTRPZ2Edition.Pages.Account;
using ChainStoreTRPZ2Edition.ViewModels.Account;

namespace ChainStoreTRPZ2Edition.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public RegisterViewModel RegisterViewModel { get; }
        public LoginViewModel LoginViewModel { get; }

        private Page _currentPage;

        public Page CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        #region NavigationCommands

        public ICommand OpenLoginPage { get; set; }
        public ICommand OpenRegisterPage { get; set; }

        #endregion


        public MainViewModel()
        {
            OpenRegisterPage = new RelayCommand(() => { CurrentPage = new RegisterPage(this); });
            OpenLoginPage = new RelayCommand(() => { CurrentPage = new LoginPage(this); });
        }

        public MainViewModel(RegisterViewModel registerViewModel, LoginViewModel loginViewModel) : this()
        {
            RegisterViewModel = registerViewModel;
            LoginViewModel = loginViewModel;
        }
    }
}