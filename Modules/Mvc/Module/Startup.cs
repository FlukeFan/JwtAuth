using System.IO;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AuthEx.Shared.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthEx.Mvc
{
    public class Startup
    {
        public Startup(IWebHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            HostEnvironment = hostEnvironment;
            Configuration = configuration;
        }

        public IWebHostEnvironment HostEnvironment { get; }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo("c:\\temp\\auth_ex_dp_keys"))
                .SetApplicationName(SecurityConstants.ApplicationName);

            var mvcBuilder = services.AddMvc();

            if (HostEnvironment.IsDevelopment())
            {
                mvcBuilder.AddRazorRuntimeCompilation();

                services.Configure<MvcRazorRuntimeCompilationOptions>(opt =>
                {
                    var libPath = Path.Combine(HostEnvironment.ContentRootPath, "..", "..", "..", "Shared");
                    var libFullPath = Path.GetFullPath(libPath);
                    opt.FileProviders.Add(new PhysicalFileProvider(libFullPath));
                });
            }

            var authority = Configuration.GetValue<string>("OidcProvider");

            services.AddAuthentication(opt =>
                {
                    opt.DefaultAuthenticateScheme = SecurityConstants.JwtScheme;
                    opt.DefaultChallengeScheme = SecurityConstants.OidcScheme;
                })
                .AddJwtBearer(SecurityConstants.JwtScheme, opt =>
                {
                    if (HostEnvironment.IsDevelopment())
                        opt.RequireHttpsMetadata = false;

                    opt.Authority = authority;
                    opt.Audience = SecurityConstants.ApplicationName;

                    opt.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            ctx.Token = ctx.Request.Cookies["JwtCookie"];
                            return Task.CompletedTask;
                        },
                    };
                })
                .AddOpenIdConnect(SecurityConstants.OidcScheme, opt =>
                {
                    if (HostEnvironment.IsDevelopment())
                        opt.RequireHttpsMetadata = false;

                    opt.Authority = authority;
                    opt.ClientId = "AuthEx";

                    // development is non-https, so allow cookies to be shared
                    if (HostEnvironment.IsDevelopment())
                    {
                        opt.CorrelationCookie.SameSite = SameSiteMode.Lax;
                        opt.NonceCookie.SameSite = SameSiteMode.Lax;
                    }

                    // sign-in is handled by storing the Jwt in a cookie
                    // in the OnTokenValidated event, so use a 'NullScheme'
                    opt.SignInScheme = "cky";

                    opt.Events.OnTokenValidated = tcv =>
                    {
                        tcv.HttpContext.Response.Cookies.Append("JwtCookie", tcv.SecurityToken.RawData);
                        return Task.CompletedTask;
                    };
                })
                .AddScheme<AuthenticationSchemeOptions, NullScheme>("cky", o => { });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (HostEnvironment.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    internal class NullScheme : AuthenticationHandler<AuthenticationSchemeOptions>, IAuthenticationSignInHandler
    {
        public NullScheme(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        public Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            return Task.CompletedTask;
        }

        public Task SignOutAsync(AuthenticationProperties properties)
        {
            return Task.CompletedTask;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return Task.FromResult(AuthenticateResult.Success(null));
        }
    }
}
