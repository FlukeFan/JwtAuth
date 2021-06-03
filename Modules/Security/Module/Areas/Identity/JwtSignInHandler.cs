using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AuthEx.Security.Lib;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthEx.Security.Areas.Identity
{
    public class JwtSignInHandler : JwtAuthenticationHandler, IAuthenticationSignInHandler
    {
        public JwtSignInHandler(IOptionsMonitor<SchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        public Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            Response.Cookies.Append("JwtCookie", "jwt-goes-here");
            return Task.CompletedTask;
        }

        public Task SignOutAsync(AuthenticationProperties properties)
        {
            throw new System.NotImplementedException();
        }
    }
}
