﻿using System;
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
using ChainStoreTRPZ2Edition.ViewModels;

namespace ChainStoreTRPZ2Edition.Pages.Account
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        public RegisterPage(MainViewModel mainViewModel)
        {
            //DataContext = mainViewModel;
            InitializeComponent();
        }
    }
}
