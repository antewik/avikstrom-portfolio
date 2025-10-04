using Microsoft.EntityFrameworkCore;
using WikstromIT.Models.Contact;

namespace WikstromIT.Data
{
    public class PortfolioDbContext : DbContext
    {
        public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options)
            : base(options)
        {
        }

        public DbSet<ContactMessage> ContactMessages { get; set; }
    }
}
