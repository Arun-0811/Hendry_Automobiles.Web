using Hendry_Auto.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hendry_Auto.Application.Contracts.Persistence
{
    public interface IVehicleTypeRepository: IGenericRepository<VehicleType>
    {
        Task Update(VehicleType vehicleType);
    }
}
