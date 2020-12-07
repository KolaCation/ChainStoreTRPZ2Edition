using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ChainStore.DataAccessLayer.Identity;
using ChainStore.DataAccessLayer.Repositories;
using ChainStore.Domain.DomainCore;
using ChainStore.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace ChainStore.DataAccessLayerImpl.Identity
{
    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly ICustomUserManager _customUserManager;
        private readonly ICustomRoleManager _customRoleManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IClientRepository _clientRepository;

        public AuthenticationService(ICustomUserManager customUserManager, ICustomRoleManager customRoleManager, IPasswordHasher<User> passwordHasher, IClientRepository clientRepository)
        {
            _customUserManager = customUserManager;
            _customRoleManager = customRoleManager;
            _passwordHasher = passwordHasher;
            _clientRepository = clientRepository;
        }

        public async Task<User> Login(string email, string password)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                var user = await _customUserManager.FindByName(email);
                if (user == null) return null;
                var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.HashedPassword, password);
                return verificationResult == PasswordVerificationResult.Success ? user : null;
            }
            else
            {
                return null;
            }
        }

        public async Task<RegistrationResult> Register(string name, string email, string password, string confirmPassword)
        {
            
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(confirmPassword))
            {
                if (password != confirmPassword) return RegistrationResult.Fail;
                var userWithProvidedEmailExists = await _customUserManager.UserExists(email);
                if (userWithProvidedEmailExists) return RegistrationResult.EmailAlreadyTaken;
                var userId = Guid.NewGuid();
                var user = new User(userId, email, email, userId);
                var hashedPassword = _passwordHasher.HashPassword(user, password);
                user.SetHashedPassword(hashedPassword);
                var clientDetails = new Client(userId, name, 0);
                await _customUserManager.CreateUser(user);
                await _clientRepository.AddOne(clientDetails);
                var roleExists = await _customRoleManager.RoleExists("Client");
                if (!roleExists) await _customRoleManager.CreateRole(new Role("Client"));
                await _customRoleManager.AddToRole(user, "Client");
                return RegistrationResult.Success;
            }
            else
            {
                return RegistrationResult.Fail;
            }
        }
    }
}
