using System;
using System.Threading;
using System.Threading.Tasks;
using AuthEx.Shared.Security;
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

        private async Task CreateUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AuthExUser>>();
            await CreateUserAsync(userManager, UserData.User.Username, UserData.User.Password);
            await CreateUserAsync(userManager, UserData.Admin.Username, UserData.Admin.Password);
        }

        private async Task CreateUserAsync(UserManager<AuthExUser> userManager, string username, string password)
        {
            var user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                user = new AuthExUser
                {
                    UserName = username,
                    Email = username,
                };

                await userManager.CreateAsync(user, password);
            }

            if (user.EmailConfirmed != true)
            {
                user.EmailConfirmed = true;
                await userManager.UpdateAsync(user);
            }
        }
    }
}
