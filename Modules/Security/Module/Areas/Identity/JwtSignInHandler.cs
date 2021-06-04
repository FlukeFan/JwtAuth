using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AuthEx.Security.Areas.Identity.Data;
using AuthEx.Security.Lib;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthEx.Security.Areas.Identity
{
    public class JwtSignInHandler : JwtAuthenticationHandler, IAuthenticationSignInHandler
    {
        private AuthExSecurityContext _db;

        public JwtSignInHandler(
            AuthExSecurityContext db,
            IOptionsMonitor<SchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _db = db;
        }

        public async Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            var key = await RsaKey.FindOrCreateAsync(_db);
            var jwt = key.CreateSignedJwt(user);
            Response.Cookies.Append("JwtCookie", jwt);
        }

        public Task SignOutAsync(AuthenticationProperties properties)
        {
            throw new System.NotImplementedException();
        }
    }
}
