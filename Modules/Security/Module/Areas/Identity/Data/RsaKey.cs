using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace AuthEx.Security.Areas.Identity.Data
{
    public class RsaKey
    {
        private const string KeyName = "AuthExKey";

        [Key]
        public string Name { get; protected set; }

        public string Xml { get; protected set; }

        public static async Task<RsaKey> FindOrCreateAsync(AuthExSecurityContext ctx)
        {
            var key = await ctx.RsaKeys
                .SingleOrDefaultAsync(k => k.Name == KeyName);

            if (key == null)
            {
                var newKey = RSA.Create(4096);
                ctx.RsaKeys.Add(new RsaKey
                {
                    Name = KeyName,
                    Xml = newKey.ToXmlString(true),
                });
            }

            return key;
        }
    }
}
