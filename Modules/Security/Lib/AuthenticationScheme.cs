using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthEx.Security.Lib
{
    public class AuthenticationScheme : SignInAuthenticationHandler<AuthenticationScheme.SchemeOptions>
    {
        public const string Name = "AuthExScheme";

        public AuthenticationScheme(IOptionsMonitor<SchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
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

        public override Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            return base.SignInAsync(user, properties);
        }

        protected override Task HandleSignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            throw new System.NotImplementedException();
        }

        protected override Task HandleSignOutAsync(AuthenticationProperties properties)
        {
            throw new System.NotImplementedException();
        }
    }
}
