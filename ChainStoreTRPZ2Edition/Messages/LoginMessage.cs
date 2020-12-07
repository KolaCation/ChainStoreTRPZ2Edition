using System;
using System.Collections.Generic;
using System.Text;

namespace ChainStoreTRPZ2Edition.Messages
{
    public sealed class LoginMessage
    {
        public bool IsLoggedIn { get; }

        public LoginMessage(bool isLoggedIn)
        {
            IsLoggedIn = isLoggedIn;
        }
    }
}
