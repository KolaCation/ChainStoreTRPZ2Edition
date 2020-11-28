using System.Windows.Controls;
using System.Windows.Input;
using ChainStoreTRPZ2Edition.Pages.Account;

namespace ChainStoreTRPZ2Edition.ViewModels
{
    public class NavigationViewModel : BaseViewModel
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

        public NavigationViewModel()
        {
            //Create command
            OpenRegisterPage = new RelayCommand(() =>
            {
                CurrentPage = new RegisterPage(this);
            });
            OpenLoginPage = new RelayCommand(() =>
            {
                CurrentPage = new LoginPage(this);
            });
        }
    }
}