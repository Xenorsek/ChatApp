using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ChatApp.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Infrastructure.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.DataAccess
{
    public class DbSeeder
    {
        public static async Task Seed(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var anyUsers = await userManager.Users.AnyAsync();
            if(!anyUsers)
            {
                await CreateRoles(serviceProvider);
                await CreateUsers(serviceProvider);
            }
        }

        private static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { Roles.User.ToString(), Roles.Administrator.ToString() };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task CreateUsers(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            List<UserAndPasswordPair> users = [
                new UserAndPasswordPair { User = new User { UserName = "user", Email = "user@gmail.com" }, Password = "#User123", Roles = [Roles.User.ToString()] },
                new UserAndPasswordPair { User = new User { UserName = "admin", Email = "admin@gmail.com" }, Password = "#Admin123", Roles = [Roles.Administrator.ToString()] },
                ];

            foreach(var user in users)
            {
                await userManager.CreateAsync(user.User, user.Password);
                if (user.Roles is not null && user.Roles.Length != 0)
                {
                    var result = await userManager.AddToRolesAsync(user.User, user.Roles);
                }
            }
        }
    }

    class UserAndPasswordPair
    {
        public required User User { get; set; }
        public required string Password { get; set; }
        public string[]? Roles { get; set; } = null;
    }
}
