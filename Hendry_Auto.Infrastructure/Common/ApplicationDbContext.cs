using Hendry_Auto.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hendry_Auto.Infrastructure.Common
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Hendry_Auto.Domain.Models.Brand> Brands { get; set; }

        public DbSet<Hendry_Auto.Domain.Models.VehicleType> VehicleTypes { get; set; }

        public DbSet<Post> Posts { get; set; }
    }
}
