using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace AuthEx.Security.Lib
{
    public static class Extensions
    {
        public static AuthenticationBuilder AddAuthExAuthentication(this IServiceCollection services)
        {
            return services
                .AddAuthentication(JwtAuthenticationHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, JwtAuthenticationHandler>(JwtAuthenticationHandler.SchemeName, o => { });
        }
    }
}
