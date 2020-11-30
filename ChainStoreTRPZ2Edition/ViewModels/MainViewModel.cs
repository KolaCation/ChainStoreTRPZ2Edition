using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using ChainStoreTRPZ2Edition.ViewModels.Account;
using DevExpress.Mvvm;

namespace ChainStoreTRPZ2Edition.ViewModels
{
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private ViewModelBase _currentViewModel;

        #region INotifyPropertyChanged Realisation
        public new event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Account.ViewModels
        public RegisterViewModel RegisterViewModel { get; }
        public LoginViewModel LoginViewModel { get; }
        #endregion


        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        #region Navigational Commands
        public ICommand OpenLoginControl { get; set; }
        public ICommand OpenRegisterControl { get; set; }
        #endregion


        public MainViewModel()
        {
            OpenLoginControl = new RelayCommand(() => { CurrentViewModel = LoginViewModel; });
            OpenRegisterControl = new RelayCommand(() => { CurrentViewModel = RegisterViewModel; });
            Messenger.Default.Register<string>(this, GotMessage);
        }

        private void GotMessage(string msg)
        {
            MessageBox.Show(msg);
        }

        public MainViewModel(RegisterViewModel registerViewModel, LoginViewModel loginViewModel) : this()
        {
            RegisterViewModel = registerViewModel;
            LoginViewModel = loginViewModel;
            CurrentViewModel = registerViewModel;
        }
    }
}