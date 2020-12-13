using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChainStoreTRPZ2Edition.Animations;

namespace ChainStoreTRPZ2Edition.UserControls.ClientOperations
{
    /// <summary>
    /// Interaction logic for PurchaseControl.xaml
    /// </summary>
    public partial class PurchaseControl : UserControl
    {
        public PurchaseControl()
        {
            InitializeComponent();
            Loaded += UserControl_Loaded;
        }

        public float SlideSeconds { get; set; } = 0.8f;

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await this.FadeIn(SlideSeconds);
        }
    }
}
