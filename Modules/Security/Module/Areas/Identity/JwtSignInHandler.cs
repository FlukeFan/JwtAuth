using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AuthEx.Security.Areas.Identity.Data;
using AuthEx.Security.Lib;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthEx.Security.Areas.Identity
{
    public class JwtSignInHandler : JwtAuthenticationHandler, IAuthenticationSignInHandler
    {
        private IConfiguration _cfg;
        private AuthExSecurityContext _db;

        public JwtSignInHandler(
            IConfiguration cfg,
            AuthExSecurityContext db,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(cfg, options, logger, encoder, clock)
        {
            _cfg = cfg;
            _db = db;
        }

        public async Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            var externalUrl = _cfg.GetValue<string>("ExternalUrl");
            var key = await RsaKey.FindOrCreateAsync(_db);
            var jwt = key.CreateSignedJwt(user, externalUrl);
            Response.Cookies.Append("JwtCookie", jwt);
        }

        public Task SignOutAsync(AuthenticationProperties properties)
        {
            throw new System.NotImplementedException();
        }
    }
}
