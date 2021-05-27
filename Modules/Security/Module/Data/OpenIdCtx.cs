using Microsoft.EntityFrameworkCore;

namespace AuthEx.Security.Data
{
    public class OpenIdCtx : DbContext
    {
        public OpenIdCtx(DbContextOptions<OpenIdCtx> options)
            : base(options)
        {
        }
    }
}
