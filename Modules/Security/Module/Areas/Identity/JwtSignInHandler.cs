using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AuthEx.Security.Areas.Identity.Data;
using AuthEx.Security.Lib;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthEx.Security.Areas.Identity
{
    public class JwtSignInHandler : AuthenticationHandler<AuthenticationSchemeOptions>, IAuthenticationSignInHandler
    {
        private IConfiguration _cfg;
        private AuthExSecurityContext _db;

        public JwtSignInHandler(
            IConfiguration cfg,
            AuthExSecurityContext db,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _cfg = cfg;
            _db = db;
        }

        public async Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            var externalUrl = _cfg.GetExternalUrl();
            var key = await RsaKey.FindOrCreateAsync(_db);
            var jwt = key.CreateSignedJwt(user, externalUrl);

            Response.Cookies.Append(JwtAuthenticationHandler.CookieName, jwt, new CookieOptions
            {
                Path = "/",
                SameSite = SameSiteMode.Strict,
            });
        }

        public Task SignOutAsync(AuthenticationProperties properties)
        {
            Response.Cookies.Delete(JwtAuthenticationHandler.CookieName);
            return Task.CompletedTask;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }
    }
}
