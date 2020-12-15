using System;
using System.Threading.Tasks;
using ChainStore.Domain.Identity;

namespace ChainStore.DataAccessLayer.Identity
{
    public interface ICustomUserManager
    {
        Task<User> FindByName(string userName);
        Task<User> FindById(Guid userId);
        Task<bool> CreateUser(User user);
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(User user);
        Task<bool> UserExists(string userName);
    }
}