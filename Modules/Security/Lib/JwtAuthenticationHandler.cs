using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthEx.Security.Lib
{
    public class JwtAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "CustomJwtScheme";

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
            var jwt = Request.Cookies["JwtCookie"];

            if (string.IsNullOrWhiteSpace(jwt))
                return AuthenticateResult.NoResult();

            var externalUrl = _cfg.GetValue<string>("ExternalUrl");

            try
            {
                if (_publicKey == null)
                {
                    var keyUri = new Uri($"{externalUrl}/Security/PublicKey");

                    using (var webClient = new WebClient())
                    {
                        var keyString = await webClient.DownloadStringTaskAsync(keyUri);
                        var keyBytes = Convert.FromBase64String(keyString);
                        var rsa = RSA.Create();
                        rsa.ImportSubjectPublicKeyInfo(keyBytes, out _);
                        _publicKey = new RsaSecurityKey(rsa);
                    }
                }

                var tokenHandler = new JwtSecurityTokenHandler();

                SecurityToken token;

                var principal = tokenHandler.ValidateToken(jwt, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = externalUrl,
                    ValidAudience = externalUrl,
                    IssuerSigningKey = _publicKey
                }, out token);

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
            Context.Response.Redirect("/Security/Identity/Account/Login");
            return Task.CompletedTask;
        }
    }
}
