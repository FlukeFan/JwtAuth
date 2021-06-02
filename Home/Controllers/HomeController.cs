using System;
using System.IdentityModel.Tokens.Jwt;
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

            var provider = new RSACryptoServiceProvider(4096);
            var key = new RsaSecurityKey(provider);

            var publicKey = provider.ExportSubjectPublicKeyInfo();
            var pub = Convert.ToBase64String(publicKey);

            var myIssuer = "http://localhost:8124";
            var myAudience = "http://localhost:8124";

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "user1"),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = myIssuer,
                Audience = myAudience,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var validJwt = tokenHandler.WriteToken(token);

            var validJwtParts = validJwt.Split('.');
            var validJwtDecoded = Encoding.ASCII.GetString(Convert.FromBase64String(validJwtParts[1]));
            var invalidJwtPart = validJwtDecoded.Replace("user1", "user2");
            var invalidJwt = $"{validJwtParts[0]}.{Convert.ToBase64String(Encoding.ASCII.GetBytes(invalidJwtPart))}.{validJwtParts[2]}";

            output += $"valid JWT=\n{validJwt}\n\n";
            output += $"invalid JWT=\n{invalidJwt}\n\n";
            output += $"pub=\n-----BEGIN PUBLIC KEY-----{pub}-----END PUBLIC KEY-----\n\n";

            return Content(output);
        }
    }
}
