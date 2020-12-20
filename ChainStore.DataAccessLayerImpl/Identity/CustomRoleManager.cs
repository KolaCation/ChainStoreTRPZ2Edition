using System;
using System.Linq;
using System.Threading.Tasks;
using ChainStore.DataAccessLayer.Identity;
using ChainStore.DataAccessLayerImpl.Helpers;
using ChainStore.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChainStore.DataAccessLayerImpl.Identity
{
    public sealed class CustomRoleManager : ICustomRoleManager
    {
        private readonly DbContextOptions<MyDbContext> _options;

        public CustomRoleManager(OptionsBuilderService<MyDbContext> optionsBuilder)
        {
            _options = optionsBuilder.BuildOptions();
        }

        public async Task<Role> FindByName(string roleName)
        {
            await using var context = new MyDbContext(_options);
            if (!string.IsNullOrEmpty(roleName))
            {
                var role = await context.Roles.FirstOrDefaultAsync(
                    e => e.RoleName.ToLower() == roleName.ToLower());
                return role;
            }

            return null;
        }

        public async Task<Role> FindById(Guid roleId)
        {
            await using var context = new MyDbContext(_options);
            if (!roleId.Equals(Guid.Empty))
            {
                var role = await context.Roles.FirstOrDefaultAsync(e => e.Id.Equals(roleId));
                return role;
            }

            return null;
        }

        public async Task<bool> CreateRole(Role role)
        {
            await using var context = new MyDbContext(_options);
            if (role != null)
            {
                await context.Roles.AddAsync(role);
                await context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateRole(Role role)
        {
            await using var context = new MyDbContext(_options);
            if (role != null)
            {
                context.Roles.Update(role);
                await context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteRole(Role role)
        {
            await using var context = new MyDbContext(_options);
            if (role != null)
            {
                context.Roles.Remove(role);
                await context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> RoleExists(string roleName)
        {
            await using var context = new MyDbContext(_options);
            if (!string.IsNullOrEmpty(roleName))
            {
                return await context.Roles.AnyAsync(e => e.RoleName.ToLower() == roleName.ToLower());
            }

            return false;
        }

        public async Task<bool> IsInRole(User user, string roleName)
        {
            await using var context = new MyDbContext(_options);
            if (user != null && !string.IsNullOrEmpty(roleName))
            {
                var userRoles = from userRole in context.UserRoles
                    join role in context.Roles on userRole.RoleId equals role.Id
                    where userRole.UserId.Equals(user.Id) && role.RoleName.ToLower() == roleName.ToLower()
                                select userRole;
                return await userRoles.CountAsync() == 1;
            }

            return false;
        }

        public async Task<bool> AddToRole(User user, string roleName)
        {
            await using var context = new MyDbContext(_options);
            if (user != null && !string.IsNullOrEmpty(roleName))
            {
                var role = await FindByName(roleName);
                var userRole = new UserRole(user.Id, role.Id);
                await context.UserRoles.AddAsync(userRole);
                await context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> RemoveFromRole(User user, string roleName)
        {
            await using var context = new MyDbContext(_options);
            if (user != null && !string.IsNullOrEmpty(roleName))
            {
                var role = await FindByName(roleName);
                var userRole =
                    await context.UserRoles.FirstOrDefaultAsync(e =>
                        e.UserId.Equals(user.Id) && e.RoleId.Equals(role.Id));
                context.UserRoles.Remove(userRole);
                await context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}