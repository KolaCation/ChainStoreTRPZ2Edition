using System;
using System.Threading.Tasks;
using ChainStore.DataAccessLayer.Identity;
using ChainStore.DataAccessLayerImpl.Helpers;
using ChainStore.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChainStore.DataAccessLayerImpl.Identity
{
    public sealed class CustomUserManager : ICustomUserManager
    {
        private readonly DbContextOptions<MyDbContext> _options;

        public CustomUserManager(OptionsBuilderService<MyDbContext> optionsBuilder)
        {
            _options = optionsBuilder.BuildOptions();
        }

        public async Task<User> FindByName(string userName)
        {
            await using var context = new MyDbContext(_options);
            if (!string.IsNullOrEmpty(userName))
            {
                var user = await context.Users.FirstOrDefaultAsync(
                    e => e.UserName.ToLower() == userName.ToLower());
                return user;
            }

            return null;
        }

        public async Task<User> FindById(Guid userId)
        {
            await using var context = new MyDbContext(_options);
            if (!userId.Equals(Guid.Empty))
            {
                var user = await context.Users.FirstOrDefaultAsync(e => e.Id.Equals(userId));
                return user;
            }

            return null;
        }

        public async Task<bool> CreateUser(User user)
        {
            await using var context = new MyDbContext(_options);
            if (user != null)
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateUser(User user)
        {
            await using var context = new MyDbContext(_options);
            if (user != null)
            {
                context.Users.Update(user);
                await context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteUser(User user)
        {
            await using var context = new MyDbContext(_options);
            if (user != null)
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> UserExists(string userName)
        {
            await using var context = new MyDbContext(_options);
            if (!string.IsNullOrEmpty(userName))
            {
                return await context.Users.AnyAsync(e => e.UserName.ToLower() == userName.ToLower());
            }

            return false;
        }
    }
}