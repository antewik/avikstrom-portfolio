using Microsoft.EntityFrameworkCore;
using AvikstromPortfolio.Models.Contact;

namespace AvikstromPortfolio.Data
{
    /// <summary>
    /// Main EF Core DbContext for the portfolio application.
    /// Contains DbSets representing database tables
    /// </summary>
    public class PortfolioDbContext : DbContext
    {
        public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options)
            : base(options)
        {
        }

        public DbSet<ContactMessage> ContactMessages { get; set; }
    }
}
