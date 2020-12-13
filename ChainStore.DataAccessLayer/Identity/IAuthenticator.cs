using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ChainStore.Domain.Identity;

namespace ChainStore.DataAccessLayer.Identity
{
    public interface IAuthenticator
    {
        bool IsLoggedIn();
        Task<bool> Login(string email, string password);
        Task<RegistrationResult> Register(string name, string email, string password, string confirmPassword);
        void Logout();
        User GetCurrentUser();
        Task<bool> CurrentUserIsInRole(string roleName);
    }
}
