using System;
using System.Collections.Generic;
using System.Text;

namespace ChainStoreTRPZ2Edition.Messages
{
    public sealed class NavigationMessage
    {
        public string ViewModelName { get; }
        public Guid? ItemId { get; }

        public NavigationMessage(string viewModelName, Guid? itemId = null)
        {
            ViewModelName = viewModelName;
            ItemId = itemId;
        }
    }
}
