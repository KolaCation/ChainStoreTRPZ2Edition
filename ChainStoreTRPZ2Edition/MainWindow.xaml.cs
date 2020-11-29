using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChainStore.DataAccessLayer.Identity;
using ChainStore.DataAccessLayerImpl.Identity;
using ChainStoreTRPZ2Edition.ViewModels;

namespace ChainStoreTRPZ2Edition
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _navigationViewModel;

        public MainWindow(MainViewModel navigationViewModel)
        {
            _navigationViewModel = navigationViewModel;
            DataContext = _navigationViewModel;
            InitializeComponent();
        }
    }
}
