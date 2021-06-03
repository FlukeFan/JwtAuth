using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthEx.Security.Lib
{
    public class JwtAuthenticationHandler : AuthenticationHandler<JwtAuthenticationHandler.SchemeOptions>
    {
        public const string SchemeName = "CustomJwtScheme";

        public class SchemeOptions : AuthenticationSchemeOptions
        {
        }

        public JwtAuthenticationHandler(IOptionsMonitor<SchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
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
    }
}
