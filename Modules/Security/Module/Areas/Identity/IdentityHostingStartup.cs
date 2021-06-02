using AuthEx.Security.Areas.Identity.Data;
using AuthEx.Security.Lib;
using AuthEx.Shared.Security;
using FileContextCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(AuthEx.Security.Areas.Identity.IdentityHostingStartup))]
namespace AuthEx.Security.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {

                services.AddDbContext<AuthExSecurityContext>(options =>
                    options.UseFileContextDatabase(location: "c:\\temp\\auth_ex_db"));

                services.AddDefaultIdentity<AuthExUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<AuthExSecurityContext>();

                services.AddHostedService<AuthExSecurityContextSetup>();

                services.AddAuthentication(options =>
                {
                    options.DefaultSignInScheme = AuthenticationScheme.Name;
                });
            });
        }
    }
}