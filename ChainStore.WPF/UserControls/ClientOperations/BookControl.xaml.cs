using System.Windows;
using System.Windows.Controls;
using ChainStoreTRPZ2Edition.Animations;

namespace ChainStoreTRPZ2Edition.UserControls.ClientOperations
{
    /// <summary>
    ///     Interaction logic for BookControl.xaml
    /// </summary>
    public partial class BookControl : UserControl
    {
        public BookControl()
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