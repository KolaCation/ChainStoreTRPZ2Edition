using System.Windows;
using System.Windows.Controls;
using ChainStoreTRPZ2Edition.Animations;

namespace ChainStoreTRPZ2Edition.UserControls.Stores
{
    /// <summary>
    ///     Interaction logic for StoresIndexControl.xaml
    /// </summary>
    public partial class StoresIndexControl : UserControl
    {
        public StoresIndexControl()
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