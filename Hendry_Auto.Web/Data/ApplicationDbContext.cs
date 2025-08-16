using Microsoft.EntityFrameworkCore;

namespace Hendry_Auto.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions <ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Hendry_Auto.Web.Models.Brand> Brands { get; set; }
    }
}
