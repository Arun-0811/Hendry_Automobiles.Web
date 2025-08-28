using Hendry_Auto.Application.Contracts.Persistence;
using Hendry_Auto.Domain.Models;
using Hendry_Auto.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hendry_Auto.Infrastructure.Repositories
{
    public class VehicleTypeRepository : GenericRepository<VehicleType>, IVehicleTypeRepository
    {
        public VehicleTypeRepository(ApplicationDbContext dbcontext) : base(dbcontext)
        {
        }

        public async Task Update(VehicleType vehicleType)
        {
            var objFrromDb = await _dbcontext.VehicleTypes.FirstOrDefaultAsync(x => x.Id == vehicleType.Id);

            if(objFrromDb != null)
            {
                objFrromDb.Name = vehicleType.Name;
            }
            _dbcontext.Update(objFrromDb);
        }
    }

}
