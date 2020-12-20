using System;
using DevExpress.Mvvm;

namespace ChainStoreTRPZ2Edition.Admin.ViewModels.Dialogs
{
    public sealed class RemoveItemViewModel : ViewModelBase
    {
        public RemoveItemViewModel(Guid itemId, string itemName)
        {
            ItemId = itemId;
            ItemName = itemName;
        }

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
    }
}