using System.IO;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AuthEx.Security.Lib;
using AuthEx.Shared.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

            services.AddAuthExAuthentication();
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
            var ticket = new AuthenticationTicket(Context.User, SecurityConstants.OidcScheme);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
