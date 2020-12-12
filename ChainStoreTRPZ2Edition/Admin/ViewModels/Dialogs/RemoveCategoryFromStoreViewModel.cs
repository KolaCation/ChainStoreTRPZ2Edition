using System;
using System.Collections.Generic;
using System.Text;
using ChainStore.Domain.DomainCore;
using ChainStoreTRPZ2Edition.DataInterfaces;
using DevExpress.Mvvm;

namespace ChainStoreTRPZ2Edition.Admin.ViewModels.Dialogs
{
    public sealed class RemoveCategoryFromStoreViewModel : ViewModelBase
    {
        public Category Category
        {
            get => GetValue<Category>();
            set => SetValue(value);
        }

        public RemoveCategoryFromStoreViewModel(Category category)
        {
            Category = category;
        }
    }
}