using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace AuthEx.Security.Areas.Identity.Data
{
    public class RsaKey
    {
        private const string KeyName = "AuthExKey";

        [Key]
        public string Name { get; protected set; }

        public string Xml { get; protected set; }

        public static async Task<RsaKey> FindOrCreateAsync(AuthExSecurityContext db)
        {
            var key = await db.RsaKeys
                .SingleOrDefaultAsync(k => k.Name == KeyName);

            if (key == null)
            {
                var newKey = RSA.Create(4096);
                db.RsaKeys.Add(new RsaKey
                {
                    Name = KeyName,
                    Xml = newKey.ToXmlString(true),
                });
            }

            return key;
        }

        public string CreateSignedJwt(ClaimsPrincipal user)
        {
            var provider = RSA.Create();
            provider.FromXmlString(Xml);

            var key = new RsaSecurityKey(provider);
            var issuer = "http://localhost:8124";
            var audience = "http://localhost:8124";

            var tokenHandler = new JwtSecurityTokenHandler();

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.FindFirst(ClaimTypes.NameIdentifier).Value),
                new Claim(JwtRegisteredClaimNames.NameId, user.Identity.Name),
            });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var jwt = tokenHandler.WriteToken(token);
            return jwt;
        }
    }
}
