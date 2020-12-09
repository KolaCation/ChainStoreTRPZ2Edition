using System;
using System.Collections.Generic;
using System.Text;

namespace ChainStoreTRPZ2Edition.Messages
{
    public sealed class RefreshDataMessage
    {
        public string ViewModelName { get; }
        public Guid? ItemId { get; }

        public RefreshDataMessage(string viewModelName, Guid? itemId = null)
        {
            ViewModelName = viewModelName;
            ItemId = itemId;
        }
    }
}
