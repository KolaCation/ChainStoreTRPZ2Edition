using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChainStore.Domain.DomainCore;
using ChainStoreTRPZ2Edition.DataInterfaces;
using DevExpress.Mvvm;

namespace ChainStoreTRPZ2Edition.Admin.ViewModels.Dialogs
{
    public sealed class AddCategoryToStoreViewModel : ViewModelBase, IVerifiable
    {
        public AddCategoryToStoreViewModel(List<Category> categories)
        {
            Categories = new ObservableCollection<Category>();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }
        }

        public ObservableCollection<Category> Categories { get; set; }

        public Category SelectedCategory
        {
            get => GetValue<Category>();
            set => SetValue(value);
        }

        public string ErrorMessage
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public bool IsValid()
        {
            if (SelectedCategory == null)
            {
                ErrorMessage = "Provide category from the list.";
                return false;
            }

            ErrorMessage = string.Empty;
            return true;
        }
    }
}