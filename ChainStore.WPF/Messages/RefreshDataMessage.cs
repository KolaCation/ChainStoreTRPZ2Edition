using System;

namespace ChainStoreTRPZ2Edition.Messages
{
    public sealed class RefreshDataMessage
    {
        public RefreshDataMessage(string viewModelName, Guid? itemId = null)
        {
            ViewModelName = viewModelName;
            ItemId = itemId;
        }

        public string ViewModelName { get; }
        public Guid? ItemId { get; }
    }
}