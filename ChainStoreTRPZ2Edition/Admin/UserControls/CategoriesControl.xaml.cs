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

namespace ChainStoreTRPZ2Edition.Admin.UserControls
{
    /// <summary>
    /// Interaction logic for CategoriesControl.xaml
    /// </summary>
    public partial class CategoriesControl : UserControl
    {
        public CategoriesControl()
        {
            InitializeComponent();
            Loaded += UserControl_Loaded;
        }

        public float SlideSeconds { get; set; } = 1f;

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await this.FadeIn(SlideSeconds);
        }
    }
}
