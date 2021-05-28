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
            using var scope = _serviceProvider.CreateScope();
            await CreateUsers(scope.ServiceProvider);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task CreateUsers(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AuthExUser>>();
            await CreateUser(userManager, UserData.User.Username, UserData.User.Password);
            await CreateUser(userManager, UserData.Admin.Username, UserData.Admin.Password);
        }

        private async Task CreateUser(UserManager<AuthExUser> userManager, string username, string password)
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
