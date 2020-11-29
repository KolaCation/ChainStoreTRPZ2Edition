using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ChainStore.DataAccessLayer.Identity;

namespace ChainStore.DataAccessLayerImpl.Identity
{
    public sealed class AuthenticationService : IAuthenticationService
    {
        public Task<bool> IsLoggedIn()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Login(string email, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Register(string email, string password, string confirmPassword)
        {
            throw new NotImplementedException();
        }
    }
}
