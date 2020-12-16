using System.Windows;
using System.Windows.Controls;
using ChainStoreTRPZ2Edition.Animations;

namespace ChainStoreTRPZ2Edition.UserControls.ClientOperations
{
    /// <summary>
    ///     Interaction logic for PurchaseControl.xaml
    /// </summary>
    public partial class PurchaseControl : UserControl
    {
        public PurchaseControl()
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