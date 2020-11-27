using System.Windows.Controls;
using System.Windows.Input;
using ChainStoreTRPZ2Edition.Pages.Account;

namespace ChainStoreTRPZ2Edition.ViewModels
{
    public sealed class MainViewModel : BaseViewModel
    {
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

 
        public ICommand OpenLoginPage { get; set; }
        public ICommand OpenRegisterPage { get; set; }

        public MainViewModel()
        {
            //Create command
            OpenLoginPage = new RelayCommand(() => {
                CurrentPage = new LoginPage();
            });
            OpenRegisterPage = new RelayCommand(() =>
            {
                CurrentPage = new RegisterPage();
            });
        }
    }
}