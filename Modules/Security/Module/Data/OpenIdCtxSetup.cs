using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuthEx.Shared.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthEx.Security.Data
{
    public class OpenIdCtxSetup : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public OpenIdCtxSetup(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            await CreateClients(scope.ServiceProvider);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task CreateClients(IServiceProvider serviceProvider)
        {
            var manager = serviceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            var client = (OpenIddictEntityFrameworkCoreApplication)await manager.FindByClientIdAsync(SecurityConstants.ApplicationName);

            if (client == null)
            {
                client = new OpenIddictEntityFrameworkCoreApplication
                {
                    ClientId = SecurityConstants.ApplicationName,
                };

                await manager.CreateAsync(client);
            }

            var redirectUris = SecurityConstants.Modules.Select(m => new Uri($"http://localhost:8124/{m}/signin-oidc"));
            client.RedirectUris = JsonConvert.SerializeObject(redirectUris);

            client.DisplayName = "Auth Example";

            var permissions = new[]
            {
                Permissions.Endpoints.Authorization,
                Permissions.GrantTypes.Implicit,
                Permissions.ResponseTypes.IdToken,
                Permissions.Scopes.Profile,

            };
            client.Permissions = JsonConvert.SerializeObject(permissions);

            await manager.UpdateAsync(client);
        }
    }
}
