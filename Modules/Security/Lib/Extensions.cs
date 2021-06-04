using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
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

        public static string GetExternalUrl(this IConfiguration cfg)
        {
            return cfg.GetValue<string>("ExternalUrl");
        }
    }
}
