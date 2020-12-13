using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Mvvm;

namespace ChainStoreTRPZ2Edition.Admin.ViewModels.Dialogs
{
    public sealed class RemoveItemViewModel : ViewModelBase
    {
        public Guid ItemId
        {
            get => GetValue<Guid>();
            set => SetValue(value);
        }

        public string ItemName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public RemoveItemViewModel(Guid itemId, string itemName)
        {
            ItemId = itemId;
            ItemName = itemName;
        }
    }
}