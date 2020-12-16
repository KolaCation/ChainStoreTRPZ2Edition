namespace ChainStoreTRPZ2Edition.Messages
{
    public sealed class LoginMessage
    {
        public LoginMessage(bool isLoggedIn)
        {
            IsLoggedIn = isLoggedIn;
        }

        public bool IsLoggedIn { get; }
    }
}