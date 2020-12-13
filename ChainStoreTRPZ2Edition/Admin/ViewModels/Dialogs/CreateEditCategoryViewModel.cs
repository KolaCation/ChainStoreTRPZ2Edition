using System;
using System.Collections.Generic;
using System.Text;
using ChainStore.Domain.DomainCore;
using ChainStoreTRPZ2Edition.DataInterfaces;
using ChainStoreTRPZ2Edition.Helpers;
using DevExpress.Mvvm;

namespace ChainStoreTRPZ2Edition.Admin.ViewModels.Dialogs
{
    public sealed class CreateEditCategoryViewModel : ViewModelBase, IVerifiable
    {
        public Guid Id { get => GetValue<Guid>(); set => SetValue(value); }
        public string Name { get=>GetValue<string>(); set=>SetValue(value); }
        public string ErrorMessage { get=>GetValue<string>(); set=>SetValue(value); }

        public CreateEditCategoryViewModel()
        {
            
        }

        public CreateEditCategoryViewModel(Category categoryToEdit)
        {
            Id = categoryToEdit.Id;
            Name = categoryToEdit.Name;
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
            else
            {
                ErrorMessage = string.Empty;
                return true;
            }
        }
    }
}
