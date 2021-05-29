using System.IO;
using AuthEx.Security.Areas.Identity.Data;
using AuthEx.Shared.Security;
using FileContextCore;
using Microsoft.AspNetCore.DataProtection;
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

                services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo("c:\\temp\\auth_ex_dp_keys"))
                    .SetApplicationName(SecurityConstants.ApplicationName);
                
                services.AddDbContext<AuthExSecurityContext>(options =>
                    options.UseFileContextDatabase(location: "c:\\temp\\auth_ex_db"));

                services.AddDefaultIdentity<AuthExUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<AuthExSecurityContext>();

                services.AddAuthentication(SecurityConstants.AuthenticationScheme);

                services.ConfigureApplicationCookie(opt =>
                {
                    opt.Cookie.Name = SecurityConstants.CookieName;
                });

                services.AddHostedService<AuthExSecurityContextSetup>();
            });
        }
    }
}