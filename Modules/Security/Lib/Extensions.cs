using Microsoft.Extensions.DependencyInjection;

namespace AuthEx.Security.Lib
{
    public static class Extensions
    {
        public static IServiceCollection AddAuthExAuthentication(this IServiceCollection services)
        {
            services
                .AddAuthentication(SimpleJwtScheme.Name)
                .AddScheme<SimpleJwtScheme.SchemeOptions, SimpleJwtScheme>(SimpleJwtScheme.Name, o => { });

            return services;
        }
    }
}
