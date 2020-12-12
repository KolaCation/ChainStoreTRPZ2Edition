using System;
using System.Collections.Generic;
using System.Text;
using ChainStore.Domain.DomainCore;
using ChainStoreTRPZ2Edition.DataInterfaces;
using ChainStoreTRPZ2Edition.Helpers;
using DevExpress.Mvvm;

namespace ChainStoreTRPZ2Edition.Admin.ViewModels.Dialogs
{
    public sealed class CreateEditStoreViewModel : ViewModelBase, IVerifiable
    {
        public Guid Id { get => GetValue<Guid>(); set => SetValue(value); }
        public string Name { get => GetValue<string>(); set => SetValue(value); }
        public string Location { get => GetValue<string>(); set => SetValue(value); }
        public string ErrorMessage { get => GetValue<string>(); set => SetValue(value); }

        public CreateEditStoreViewModel()
        {

        }

        public CreateEditStoreViewModel(Store storeToEdit)
        {
            Id = storeToEdit.Id;
            Name = storeToEdit.Name;
            Location = storeToEdit.Location;
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
            else if (string.IsNullOrEmpty(Location))
            {
                ErrorMessage = ErrorMessages.Required(nameof(Location));
                return false;
            }
            else if (Location.Length < 2)
            {
                ErrorMessage = ErrorMessages.StringMinLength(nameof(Location), 2);
                return false;
            }
            else if (Location.Length > 60)
            {
                ErrorMessage = ErrorMessages.StringMaxLength(nameof(Location), 60);
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