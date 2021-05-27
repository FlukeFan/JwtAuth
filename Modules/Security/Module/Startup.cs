using System;
using System.IO;
using AuthEx.Security.Data;
using FileContextCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using ZipDeploy;

namespace AuthEx.Security
{
    public class Startup
    {
        public Startup(IWebHostEnvironment hostEnvironment)
        {
            HostEnvironment = hostEnvironment;
        }

        public IWebHostEnvironment HostEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddZipDeploy();

            services.AddSignalR();

            services.AddDbContext<OpenIdCtx>(options =>
            {
                options.UseFileContextDatabase(location: "c:\\temp\\auth_ex_openid_db");
                options.UseOpenIddict();
            });

            var mvcBuilder = services.AddRazorPages();

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

            services.AddOpenIddict(opt =>
            {
                opt.AddCore(o => o.UseEntityFrameworkCore().UseDbContext<OpenIdCtx>());

                opt.AddServer(options =>
                {
                    // Enable the authorization, logout, token and userinfo endpoints.
                    options.SetAuthorizationEndpointUris("/connect/authorize");

                    // Mark the "email", "profile" and "roles" scopes as supported scopes.
                    //options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);

                    // Note: this sample only uses the authorization code flow but you can enable
                    // the other flows if you need to support implicit, password or client credentials.
                    options.AllowImplicitFlow();

                    options.AddEncryptionKey(new SymmetricSecurityKey(
                        Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));

                    // Register the signing and encryption credentials.
                    options.AddDevelopmentSigningCertificate();

                    // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                    options.UseAspNetCore()
                        .DisableTransportSecurityRequirement();
                });
            });

            services.AddHostedService<OpenIdCtxSetup>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
