using Microsoft.EntityFrameworkCore;

namespace AddWatermarksToImages.Models
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions):base(dbContextOptions)
        {
            
        }
    }
}
