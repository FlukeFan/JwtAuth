using System;
using System.Threading;
using System.Threading.Tasks;
using AuthEx.Shared.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;

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

            if (await manager.FindByClientIdAsync(SecurityConstants.ApplicationName) == null)
            {
                var descriptor = new OpenIddictApplicationDescriptor
                {
                    ClientId = SecurityConstants.ApplicationName,
                    DisplayName = "Auth Example",
                    RedirectUris =
                        {
                            new Uri("http://localhost:8124/*/signin-oidc")
                        },
                };

                await manager.CreateAsync(descriptor);
            }
        }
    }
}
