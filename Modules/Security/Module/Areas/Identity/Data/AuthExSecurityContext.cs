using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthEx.Security.Areas.Identity.Data
{
    public class AuthExSecurityContext : IdentityDbContext<IdentityUser>
    {
        public AuthExSecurityContext(DbContextOptions<AuthExSecurityContext> options)
            : base(options)
        {
        }

        public virtual DbSet<RsaKey> RsaKeys { get; set; }
    }
}
