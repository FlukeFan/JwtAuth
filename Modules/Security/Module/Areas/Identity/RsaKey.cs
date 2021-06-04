using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AuthEx.Security.Areas.Identity.Data;
using Microsoft.IdentityModel.Tokens;

namespace AuthEx.Security.Areas.Identity
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

        public string EncodePublicKey()
        {
            var provider = CreateProvider();
            var publicKey = provider.ExportSubjectPublicKeyInfo();
            var encoded = Convert.ToBase64String(publicKey);
            return encoded;
        }

        public string CreateSignedJwt(ClaimsPrincipal user, string externalUrl)
        {
            var provider = CreateProvider();

            var key = new RsaSecurityKey(provider);

            var tokenHandler = new JwtSecurityTokenHandler();

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.FindFirst(ClaimTypes.NameIdentifier).Value),
                new Claim(ClaimTypes.Name, user.Identity.Name),
            });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = externalUrl,
                Audience = externalUrl,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var jwt = tokenHandler.WriteToken(token);
            return jwt;
        }

        private RSA CreateProvider()
        {
            var provider = RSA.Create();
            provider.FromXmlString(Xml);
            return provider;
        }
    }
}
