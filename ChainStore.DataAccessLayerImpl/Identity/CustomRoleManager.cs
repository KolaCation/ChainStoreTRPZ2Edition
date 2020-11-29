﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChainStore.DataAccessLayer.Identity;
using ChainStore.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ChainStore.DataAccessLayerImpl.Identity
{
    public sealed class CustomRoleManager : ICustomRoleManager
    {
        private readonly MyDbContext _context;

        public CustomRoleManager(MyDbContext context)
        {
            _context = context;
        }


        public async Task<Role> FindByName(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var role = await _context.Roles.FirstOrDefaultAsync(
                    e => e.RoleName.ToLower().Equals(roleName.ToLower()));
                return role;
            }
            else
            {
                return null;
            }
        }

        public async Task<Role> FindById(Guid roleId)
        {
            if (!roleId.Equals(Guid.Empty))
            {
                var role = await _context.Roles.FirstOrDefaultAsync(e => e.Id.Equals(roleId));
                return role;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> CreateRole(Role role)
        {
            if (role != null)
            {
                await _context.Roles.AddAsync(role);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UpdateRole(Role role)
        {
            if (role != null)
            {
                _context.Roles.Update(role);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteRole(Role role)
        {
            if (role != null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> RoleExists(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                return await _context.Roles.AnyAsync(e => e.RoleName.ToLower().Equals(roleName.ToLower()));
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> IsInRole(User user, string roleName)
        {
            if (user != null && !string.IsNullOrEmpty(roleName))
            {
                var userRoles = from userRole in _context.UserRoles
                    join role in _context.Roles on userRole.RoleId equals role.Id
                    where userRole.UserId.Equals(user.Id) && role.RoleName.ToLower().Equals(roleName.ToLower())
                    select userRole;
                return await userRoles.CountAsync() == 1;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> AddToRole(User user, string roleName)
        {
            if (user != null && !string.IsNullOrEmpty(roleName))
            {
                var role = await FindByName(roleName);
                var userRole = new UserRole(user.Id, role.Id);
                await _context.UserRoles.AddAsync(userRole);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> RemoveFromRole(User user, string roleName)
        {
            if (user != null && !string.IsNullOrEmpty(roleName))
            {
                var role = await FindByName(roleName);
                var userRole = await _context.UserRoles.FirstOrDefaultAsync(e => e.UserId.Equals(user.Id) && e.RoleId.Equals(role.Id));
                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}