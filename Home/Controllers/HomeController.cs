using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
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

            var provider = new RSACryptoServiceProvider(2048);
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
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            output += $"JWT=\n{tokenHandler.WriteToken(token)}\n\n";
            output += $"pub=\n-----BEGIN PUBLIC KEY-----{pub}-----END PUBLIC KEY-----\n\n";

            return Content(output);
        }
    }
}
