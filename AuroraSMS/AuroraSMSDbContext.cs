using Microsoft.EntityFrameworkCore;

namespace AuroraSMS
{
    public class AuroraSMSDbContext : DbContext
    {
        public AuroraSMSDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AuroraSmsMessage> Messages { get; set; }
    }
}
