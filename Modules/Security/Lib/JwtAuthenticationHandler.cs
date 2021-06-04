using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthEx.Security.Lib
{
    public class JwtAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "CustomJwtScheme";
        public const string CookieName = "JwtCookie";

        private static RsaSecurityKey _publicKey;

        private IConfiguration _cfg;

        public JwtAuthenticationHandler(
            IConfiguration cfg,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _cfg = cfg;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var jwt = Request.Cookies[CookieName];

            if (string.IsNullOrWhiteSpace(jwt))
                return AuthenticateResult.NoResult();

            var externalUrl = _cfg.GetValue<string>("ExternalUrl");

            try
            {
                _publicKey = _publicKey ?? await DownloadPublicKeyAsync(externalUrl);

                var tokenHandler = new JwtSecurityTokenHandler();

                var principal = tokenHandler.ValidateToken(jwt, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = externalUrl,
                    ValidAudience = externalUrl,
                    IssuerSigningKey = _publicKey
                }, out _);

                var ticket = new AuthenticationTicket(principal, SchemeName);
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error authenticating JWT");
                return AuthenticateResult.Fail(ex);
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var redirectUri = OriginalPathBase + OriginalPath + Request.QueryString;
            var loginUri = "/Security/Identity/Account/Login" + QueryString.Create("returnUrl", redirectUri);
            Context.Response.Redirect(loginUri);
            return Task.CompletedTask;
        }

        private static async Task<RsaSecurityKey> DownloadPublicKeyAsync(string externalUrl)
        {
            var keyUri = new Uri($"{externalUrl}/Security/PublicKey");

            using (var webClient = new WebClient())
            {
                var keyString = await webClient.DownloadStringTaskAsync(keyUri);
                var keyBytes = Convert.FromBase64String(keyString);
                var rsa = RSA.Create();
                rsa.ImportSubjectPublicKeyInfo(keyBytes, out _);
                
                return new RsaSecurityKey(rsa);
            }
        }
    }
}
