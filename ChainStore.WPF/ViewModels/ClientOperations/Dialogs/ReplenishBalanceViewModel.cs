﻿using ChainStoreTRPZ2Edition.DataInterfaces;
using ChainStoreTRPZ2Edition.Helpers;
using DevExpress.Mvvm;

namespace ChainStoreTRPZ2Edition.ViewModels.ClientOperations.Dialogs
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

        public bool IsValid()
        {
            if (Balance < 10)
            {
                ErrorMessage = ErrorMessages.MinValue("Sum to replenish", 10) + " UAH";
                return false;
            }

            if (Balance > 100_000_000)
            {
                ErrorMessage = ErrorMessages.MaxValue("Sum to replenish", 100_000_000) + " UAH";
                return false;
            }

            ErrorMessage = string.Empty;
            return true;
        }
    }
}