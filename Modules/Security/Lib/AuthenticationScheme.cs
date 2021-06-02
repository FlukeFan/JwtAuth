using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthEx.Security.Lib
{
    public class AuthenticationScheme : AuthenticationHandler<AuthenticationScheme.SchemeOptions>
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
    }
}
