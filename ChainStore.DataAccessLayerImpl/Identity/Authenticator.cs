﻿using System.Threading.Tasks;
using ChainStore.DataAccessLayer.Identity;
using ChainStore.Domain.Identity;

namespace ChainStore.DataAccessLayerImpl.Identity
{
    public sealed class Authenticator : IAuthenticator
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ICustomRoleManager _customRoleManager;

        public Authenticator(IAuthenticationService authenticationService, ICustomRoleManager customRoleManager)
        {
            _authenticationService = authenticationService;
            _customRoleManager = customRoleManager;
        }

        public User CurrentUser { get; private set; }

        public bool IsLoggedIn()
        {
            return CurrentUser != null;
        }

        public async Task<bool> Login(string email, string password)
        {
            var user = await _authenticationService.Login(email, password);
            CurrentUser = user;
            return CurrentUser != null;
        }

        public async Task<RegistrationResult> Register(string name, string email, string password,
            string confirmPassword)
        {
            return await _authenticationService.Register(name, email, password, confirmPassword);
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        public User GetCurrentUser()
        {
            return CurrentUser;
        }

        public async Task<bool> CurrentUserIsInRole(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName) && IsLoggedIn())
            {
                return await _customRoleManager.IsInRole(CurrentUser, roleName);
            }

            return false;
        }
    }
}