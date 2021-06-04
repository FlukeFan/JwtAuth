using System.Threading.Tasks;
using AuthEx.Security.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace AuthEx.Security.Areas.Identity
{
    public class PublicKeyController : Controller
    {
        private AuthExSecurityContext _db;

        public PublicKeyController(AuthExSecurityContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(bool delimit = false)
        {
            var key = await RsaKey.FindOrCreateAsync(_db);
            var encodedPublicKey = key.EncodePublicKey();

            if (delimit)
                encodedPublicKey = $"-----BEGIN PUBLIC KEY-----{encodedPublicKey}-----END PUBLIC KEY-----";

            return Content(encodedPublicKey);
        }
    }
}
