using System;
using System.Collections.Generic;
using System.Text;
using ChainStoreTRPZ2Edition.DataInterfaces;
using DevExpress.Mvvm;

namespace ChainStoreTRPZ2Edition.ViewModels.Dialogs
{
    public sealed class ReplenishBalanceViewModel : ViewModelBase, IVerifiable
    {
        public double Balance
        {
            get => GetValue<double>();
            set => SetValue(value);
        }

        public string ErrorMessage
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public ReplenishBalanceViewModel()
        {
        }

        public bool IsValid()
        {
            return false;
        }
    }
}