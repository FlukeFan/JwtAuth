using Microsoft.Extensions.DependencyInjection;

namespace AuthEx.Security.Lib
{
    public static class Extensions
    {
        public static IServiceCollection AddAuthExAuthentication(this IServiceCollection services)
        {
            services
                .AddAuthentication(AuthenticationScheme.Name)
                .AddScheme<AuthenticationScheme.SchemeOptions, AuthenticationScheme>(AuthenticationScheme.Name, o => { });

            return services;
        }
    }
}
