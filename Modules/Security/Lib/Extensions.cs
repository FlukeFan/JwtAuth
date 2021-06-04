using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace AuthEx.Security.Lib
{
    public static class Extensions
    {
        public static IServiceCollection AddAuthExAuthentication(this IServiceCollection services)
        {
            services
                .AddAuthentication(JwtAuthenticationHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, JwtAuthenticationHandler>(JwtAuthenticationHandler.SchemeName, o => { });

            return services;
        }
    }
}
