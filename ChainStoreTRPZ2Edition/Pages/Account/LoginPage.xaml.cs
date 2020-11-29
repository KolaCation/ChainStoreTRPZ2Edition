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

namespace ChainStoreTRPZ2Edition
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private readonly NavigationViewModel _navigationViewModel;
        public LoginPage(NavigationViewModel navigationViewModel)
        {
            _navigationViewModel = navigationViewModel;
            InitializeComponent();
        }

        private void NavigateToSignUp(object sender, RoutedEventArgs e)
        {
            _navigationViewModel.OpenRegisterPage.Execute(null);
        }
    }
}