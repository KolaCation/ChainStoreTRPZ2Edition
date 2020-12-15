using System;
using System.Threading.Tasks;
using ChainStore.DataAccessLayer.Identity;
using ChainStore.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChainStore.DataAccessLayerImpl.Identity
{
    public sealed class CustomUserManager : ICustomUserManager
    {
        private readonly MyDbContext _context;

        public CustomUserManager(MyDbContext context)
        {
            _context = context;
        }

        public async Task<User> FindByName(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var user = await _context.Users.FirstOrDefaultAsync(
                    e => e.UserName.ToLower().Equals(userName.ToLower()));
                return user;
            }

            return null;
        }

        public async Task<User> FindById(Guid userId)
        {
            if (!userId.Equals(Guid.Empty))
            {
                var user = await _context.Users.FirstOrDefaultAsync(e => e.Id.Equals(userId));
                return user;
            }

            return null;
        }

        public async Task<bool> CreateUser(User user)
        {
            if (user != null)
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateUser(User user)
        {
            if (user != null)
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteUser(User user)
        {
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> UserExists(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
                return await _context.Users.AnyAsync(e => e.UserName.ToLower().Equals(userName.ToLower()));
            return false;
        }
    }
}