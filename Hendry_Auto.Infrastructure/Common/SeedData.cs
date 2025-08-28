using Hendry_Auto.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hendry_Auto.Infrastructure.Common
{
    public class SeedData
    {
        public static async Task SeedDataAsync( ApplicationDbContext _dbcontext)
        {
            if(!_dbcontext.VehicleTypes.Any())
            {
                await _dbcontext.VehicleTypes.AddRangeAsync(
                new VehicleType
                { 
                    Name = "Car" 
                }, 
                new VehicleType 
                { 
                    Name = "Truck" 
                }, 
                new VehicleType 
                { 
                    Name = "SUV" 
                },
                new VehicleType
                {
                    Name = "Motorcycle"
                },
                new VehicleType
                {
                    Name = "Van"
                },
                new VehicleType
                {
                    Name = "Sedan"
                });
                await _dbcontext.SaveChangesAsync();
            }
        } 
    }
}
