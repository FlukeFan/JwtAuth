using AuthEx.Shared.Security;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthEx.Security.Areas.Identity.Data
{
    public class AuthExSecurityContext : IdentityDbContext<AuthExUser>
    {
        public AuthExSecurityContext(DbContextOptions<AuthExSecurityContext> options)
            : base(options)
        {
        }

        public virtual DbSet<RsaKey> RsaKeys { get; set; }
    }
}
