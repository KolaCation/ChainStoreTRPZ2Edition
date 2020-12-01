using System;
using System.Collections.Generic;
using System.Text;

namespace ChainStoreTRPZ2Edition.Messages
{
    public sealed class RefreshDataMessage
    {
        public string ViewModelName { get; set; }

        public RefreshDataMessage()
        {

        }
        public RefreshDataMessage(string viewModelName)
        {
            ViewModelName = viewModelName;
        }
    }
}
