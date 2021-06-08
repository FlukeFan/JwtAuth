using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AuthEx.Security.Areas.Identity.Data
{
    public class AuthExSecurityContextSetup : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public AuthExSecurityContextSetup(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                await CreateKeyAsync(scope.ServiceProvider);
                await CreateRolesAsync(scope.ServiceProvider, UserData.Roles.Users, UserData.Roles.Admins);
                await CreateUsersAsync(scope.ServiceProvider);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task CreateKeyAsync(IServiceProvider serviceProvider)
        {
            var ctx = serviceProvider.GetRequiredService<AuthExSecurityContext>();
            await RsaKey.FindOrCreateAsync(ctx);
            await ctx.SaveChangesAsync();
        }

        private async Task CreateRolesAsync(IServiceProvider serviceProvider, params string[] roles)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var existingRoles = roleManager.Roles.AsQueryable().Select(r => r.Name).ToList();

            foreach (var role in roles)
                if (!existingRoles.Contains(role))
                    await roleManager.CreateAsync(new IdentityRole
                    {
                        Name = role,
                    });
        }

        private async Task CreateUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            await CreateUserAsync(userManager, UserData.User.Username, UserData.User.Password, UserData.Roles.Users);
            await CreateUserAsync(userManager, UserData.Admin.Username, UserData.Admin.Password, UserData.Roles.Users, UserData.Roles.Admins);
        }

        private async Task CreateUserAsync(UserManager<IdentityUser> userManager, string username, string password, params string[] roles)
        {
            var user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = username,
                    Email = username,
                };

                await userManager.CreateAsync(user, password);
            }

            if (!await userManager.CheckPasswordAsync(user, password))
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                await userManager.ResetPasswordAsync(user, token, password);
            }

            if (user.EmailConfirmed != true)
            {
                user.EmailConfirmed = true;
                await userManager.UpdateAsync(user);
            }

            var desiredRoles = roles ?? new string[0];
            var existingRoles = await userManager.GetRolesAsync(user);

            foreach (var roleToAdd in desiredRoles.Where(r => !existingRoles.Contains(r)))
                await userManager.AddToRoleAsync(user, roleToAdd);

            foreach (var roleToRemove in existingRoles.Where(r => !desiredRoles.Contains(r)))
                await userManager.RemoveFromRoleAsync(user, roleToRemove);
        }
    }
}
