using System.IO;
using System.Threading.Tasks;
using AuthEx.Shared.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

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

                    opt.SignInScheme = SecurityConstants.JwtScheme;
                });
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
}
