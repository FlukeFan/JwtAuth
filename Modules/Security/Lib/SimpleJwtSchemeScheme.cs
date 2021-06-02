using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthEx.Security.Lib
{
    public class SimpleJwtScheme : SignInAuthenticationHandler<SimpleJwtScheme.SchemeOptions>
    {
        public const string Name = "SimpleJwtScheme";

        public SimpleJwtScheme(IOptionsMonitor<SchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        public class SchemeOptions : AuthenticationSchemeOptions
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            await Task.CompletedTask;
            return AuthenticateResult.NoResult();
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Context.Response.Redirect("/Security/Identity/Account/Login");
            return Task.CompletedTask;
        }

        protected override Task HandleSignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            Response.Cookies.Append("JwtCookie", "jwt-goes-here");
            return Task.CompletedTask;
        }

        protected override Task HandleSignOutAsync(AuthenticationProperties properties)
        {
            Context.Response.Cookies.Delete("JwtCookie");
            return Task.CompletedTask;
        }
    }
}
