using AuthEx.Security.Areas.Identity.Data;
using AuthEx.Shared;
using FileContextCore;
using Microsoft.AspNetCore.Authentication;
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
                    options.UseFileContextDatabase(location: context.HostingEnvironment.AppFolder("auth_ex_security_db")));

                services.AddIdentityCore<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddDefaultUI()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<AuthExSecurityContext>()
                    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>(TokenOptions.DefaultProvider); ;

                services.AddHostedService<AuthExSecurityContextSetup>();

                services.AddAuthentication()
                    .AddScheme<AuthenticationSchemeOptions, JwtSignInHandler>(IdentityConstants.ApplicationScheme, o => { })
                    .AddCookie(IdentityConstants.ExternalScheme)
                    .AddCookie(IdentityConstants.TwoFactorUserIdScheme);
            });
        }
    }
}