using AuthEx.Security.Areas.Identity.Data;
using AuthEx.Security.Lib;
using AuthEx.Shared.Security;
using FileContextCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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

                services.AddIdentityCore<AuthExUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddDefaultUI()
                    .AddEntityFrameworkStores<AuthExSecurityContext>();

                services.AddHostedService<AuthExSecurityContextSetup>();

                services.AddAuthentication()
                    .AddScheme<SimpleJwtScheme.SchemeOptions, SimpleJwtScheme>(IdentityConstants.ApplicationScheme, o => { })
                    .AddCookie(IdentityConstants.ExternalScheme);
            });
        }
    }
}