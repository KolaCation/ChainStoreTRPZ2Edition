using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ChainStoreTRPZ2Edition.Animations;

namespace ChainStoreTRPZ2Edition.UserControls.Account
{
    /// <summary>
    /// Interaction logic for LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        public LoginControl()
        {
            InitializeComponent();
            Loaded += UserControl_Loaded;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await this.FadeIn();
        }
    }
}