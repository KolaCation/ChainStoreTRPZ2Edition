using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ChainStore.DataAccessLayer.Identity;
using ChainStore.Domain.Identity;

namespace ChainStore.DataAccessLayerImpl.Identity
{
    public sealed class Authenticator : IAuthenticator
    {
        private readonly IAuthenticationService _authenticationService;

        public User CurrentUser { get; private set; }

        public Authenticator(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public bool IsLoggedIn()
        {
            return CurrentUser == null;
        }

        public async Task<bool> Login(string email, string password)
        {
            var user = await _authenticationService.Login(email, password);
            CurrentUser = user;
            return CurrentUser == null;
        }

        public async Task<bool> Register(string name, string email, string password, string confirmPassword)
        {
            return await _authenticationService.Register(name, email, password, confirmPassword);
        }

        public void Logout()
        {
            CurrentUser = null;
        }
    }
}
