using Hendry_Auto.Application.Contracts.Persistence;
using Hendry_Auto.Infrastructure.Common;
using Hendry_Auto.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hendry_Auto.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly ApplicationDbContext _dbcontext;
        public UnitOfWork(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
            Brand = new BrandRepository(_dbcontext);
            VehicleType = new VehicleTypeRepository(_dbcontext);
        }
        public IBrandRepository Brand { get; set; }

        public IVehicleTypeRepository VehicleType { get; private set; }

        public void Dispose()
        {
            _dbcontext.Dispose();
        }

        public async Task SaveAsync()
        {
            await _dbcontext.SaveChangesAsync();
        }
    }
}
