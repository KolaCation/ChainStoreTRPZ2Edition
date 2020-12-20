using System;

namespace ChainStoreTRPZ2Edition.Messages
{
    public sealed class NavigationMessage
    {
        public NavigationMessage(string viewModelName, Guid? itemId = null)
        {
            ViewModelName = viewModelName;
            ItemId = itemId;
        }

        public string ViewModelName { get; }
        public Guid? ItemId { get; }
    }
}