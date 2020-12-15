using System;
using System.Threading.Tasks;
using ChainStore.Domain.Identity;

namespace ChainStore.DataAccessLayer.Identity
{
    public interface ICustomRoleManager
    {
        Task<Role> FindByName(string roleName);
        Task<Role> FindById(Guid roleId);
        Task<bool> CreateRole(Role role);
        Task<bool> UpdateRole(Role role);
        Task<bool> DeleteRole(Role role);
        Task<bool> RoleExists(string roleName);
        Task<bool> IsInRole(User user, string roleName);
        Task<bool> AddToRole(User user, string roleName);
        Task<bool> RemoveFromRole(User user, string roleName);
    }
}