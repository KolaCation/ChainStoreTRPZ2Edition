using System;
using System.Collections.Generic;
using System.Text;

namespace ChainStoreTRPZ2Edition.Messages
{
    public sealed class NavigationMessage
    {
        public string ViewModelName { get; set; }

        public NavigationMessage()
        {
            
        }
        public NavigationMessage(string viewModelName)
        {
            ViewModelName = viewModelName;
        }
    }
}
