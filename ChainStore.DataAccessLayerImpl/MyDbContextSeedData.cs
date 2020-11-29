using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ChainStore.DataAccessLayer.Identity;
using ChainStore.DataAccessLayerImpl.DbModels;
using ChainStore.Domain.Identity;

namespace ChainStore.DataAccessLayerImpl
{
    public static class MyDbContextSeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<MyDbContext>();
                var userManager = scope.ServiceProvider.GetService<ICustomUserManager>();
                var roleManager = scope.ServiceProvider.GetService<ICustomRoleManager>();


                var email = configuration.GetValue<string>("AdminUserEmail");
                var password = configuration.GetValue<string>("AdminUserPswd");
                var passwordHasher = new PasswordHasher<User>();
                var adminId = Guid.NewGuid();

                var user = new User(adminId, email.ToLower(), email.ToLower(), adminId);
                var hashedPassword = passwordHasher.HashPassword(user, password);
                user.SetHashedPassword(hashedPassword);
                var userExists = await userManager.UserExists(email.ToLower());

                if (!userExists)
                {
                    var userCreationSucceeded = await userManager.CreateUser(user);
                    if (userCreationSucceeded)
                    {
                        var adminRole = new Role("Admin");
                        var roleExists = await roleManager.RoleExists(adminRole.RoleName);
                        if (!roleExists)
                        {
                            var roleCreationSucceeded = await roleManager.CreateRole(adminRole);
                            if (roleCreationSucceeded)
                            {
                                var adminUser = await userManager.FindById(adminId);
                                await roleManager.AddToRole(adminUser, adminRole.RoleName);
                            }
                        }
                        try
                        {
                            await context.Clients.AddAsync(new ClientDbModel(adminId, "Husk", 0));
                            await context.SaveChangesAsync();
                        }
                        catch (Exception)
                        {
                            throw new ApplicationException();
                        }
                    }
                }
            }
        }
    }
}