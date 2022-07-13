using Microsoft.EntityFrameworkCore;

namespace RabbitMQAddWatermarksToImages.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }
    }
}
