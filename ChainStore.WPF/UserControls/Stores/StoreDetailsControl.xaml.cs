using System.Windows;
using System.Windows.Controls;
using ChainStoreTRPZ2Edition.Animations;

namespace ChainStoreTRPZ2Edition.UserControls.Stores
{
    /// <summary>
    ///     Interaction logic for StoreDetailsControl.xaml
    /// </summary>
    public partial class StoreDetailsControl : UserControl
    {
        public StoreDetailsControl()
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