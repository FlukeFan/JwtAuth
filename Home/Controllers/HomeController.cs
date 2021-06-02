using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthEx.Home.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Page1()
        {
            return View();
        }

        [Authorize]
        public IActionResult Page2()
        {
            return View();
        }

        public IActionResult Jwt()
        {
            var output = "JWT\n\n";

            var provider = GetProvider();
            var key = new RsaSecurityKey(provider);

            var publicKey = provider.ExportSubjectPublicKeyInfo();
            var pub = Convert.ToBase64String(publicKey);

            var issuer = "http://localhost:8124";
            var audience = "http://localhost:8124";

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "user1"),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var validJwt = tokenHandler.WriteToken(token);

            var validJwtParts = validJwt.Split('.');
            var validJwtDecoded = Encoding.ASCII.GetString(Convert.FromBase64String(validJwtParts[1]));
            var invalidJwtPart = validJwtDecoded.Replace("user1", "user2");
            var invalidJwt = $"{validJwtParts[0]}.{Convert.ToBase64String(Encoding.ASCII.GetBytes(invalidJwtPart))}.{validJwtParts[2]}";

            output += $"pub=\n-----BEGIN PUBLIC KEY-----{pub}-----END PUBLIC KEY-----\n\n";
            output += $"valid JWT=\n{validJwt}\n\n";
            output += $"invalid JWT=\n{invalidJwt}\n\n";

            output += $"validate(valid JTW)={Validate(validJwt, issuer, audience, key)}\n";
            output += $"validate(invalid JTW)={Validate(invalidJwt, issuer, audience, key)}\n\n";

            output += $"claims: {string.Join("; ", Claims(validJwt))}\n\n";

            return Content(output);
        }

        private RSACryptoServiceProvider GetProvider()
        {
            var provider = new RSACryptoServiceProvider(4096);

            if (!System.IO.File.Exists("params.txt"))
                System.IO.File.WriteAllText("params.txt", provider.ToXmlString(true));

            provider.FromXmlString(System.IO.File.ReadAllText("params.txt"));
            return provider;
        }

        private string Validate(string jwt, string issuer, string audience, SecurityKey key)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(jwt, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = key
                }, out _);
            }
            catch (Exception ex)
            {
                return "False: " + ex.Message.Replace("\n", "; ");
            }
            return "True";
        }

        private string[] Claims(string jwt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(jwt) as JwtSecurityToken;
            return securityToken.Claims.Select(c => $"{c.Type}:{c.Value}").ToArray();
        }
    }
}
