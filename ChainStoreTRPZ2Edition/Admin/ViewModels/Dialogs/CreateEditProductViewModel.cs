using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using ChainStore.Domain.DomainCore;
using ChainStoreTRPZ2Edition.DataInterfaces;
using ChainStoreTRPZ2Edition.Helpers;
using DevExpress.Mvvm;

namespace ChainStoreTRPZ2Edition.Admin.ViewModels.Dialogs
{
    public sealed class CreateEditProductViewModel : ViewModelBase, IVerifiable
    {
        public Guid Id
        {
            get => GetValue<Guid>();
            set => SetValue(value);
        }

        public string Name
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public double PriceInUAH
        {
            get => GetValue<double>();
            set => SetValue(value);
        }

        public Guid CategoryId
        {
            get => GetValue<Guid>();
            set => SetValue(value);
        }

        public int QuantityOfProducts
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public string ErrorMessage
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public bool NameTextBoxEnabled
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        public bool PriceTextBoxEnabled
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        public bool QuantityOfProductsTextBoxEnabled
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        public ProductOperationType Type { get=>GetValue<ProductOperationType>();
            private set=>SetValue(value); }

        public ProductStatus ProductStatus { get; private set; }

        public CreateEditProductViewModel(ProductOperationType type, Guid categoryId)
        {
            EnableTextBoxesDependingOnOperationType(type);
            Type = type;
            CategoryId = categoryId;
            ProductStatus = ProductStatus.OnSale;
        }

        public CreateEditProductViewModel(ProductOperationType type, Product product)
        {
            EnableTextBoxesDependingOnOperationType(type);
            Type = type;
            Id = product.Id;
            Name = product.Name;
            PriceInUAH = product.PriceInUAH;
            ProductStatus = product.ProductStatus;
            CategoryId = product.CategoryId;
        }

        private void EnableTextBoxesDependingOnOperationType(ProductOperationType type)
        {
            if (type == ProductOperationType.Create)
            {
                NameTextBoxEnabled = true;
                PriceTextBoxEnabled = true;
                QuantityOfProductsTextBoxEnabled = true;
            }
            else if (type == ProductOperationType.Edit)
            {
                NameTextBoxEnabled = true;
                PriceTextBoxEnabled = true;
                QuantityOfProductsTextBoxEnabled = false;
            }
            else if (type == ProductOperationType.Replenish)
            {
                NameTextBoxEnabled = false;
                PriceTextBoxEnabled = false;
                QuantityOfProductsTextBoxEnabled = true;
            }
        }


        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Name))
            {
                ErrorMessage = ErrorMessages.Required(nameof(Name));
                return false;
            }
            else if (Name.Length < 2)
            {
                ErrorMessage = ErrorMessages.StringMinLength(nameof(Name), 2);
                return false;
            }
            else if (Name.Length > 60)
            {
                ErrorMessage = ErrorMessages.StringMaxLength(nameof(Name), 60);
                return false;
            }
            else if (PriceInUAH < 10)
            {
                ErrorMessage = ErrorMessages.MinValue("Price", 10) + " UAH";
                return false;
            }
            else if (PriceInUAH > 100_000_000)
            {
                ErrorMessage = ErrorMessages.MaxValue("Price", 100_000_000) + " UAH";
                return false;
            }
            else if (QuantityOfProducts < 1 && Type != ProductOperationType.Edit)
            {
                ErrorMessage = ErrorMessages.MinValue("Quantity of products", 1);
                return false;
            }
            else if (QuantityOfProducts > 100 && Type != ProductOperationType.Edit)
            {
                ErrorMessage = ErrorMessages.MaxValue("Quantity of products", 100);
                return false;
            }
            else
            {
                ErrorMessage = string.Empty;
                return true;
            }
        }
    }
}