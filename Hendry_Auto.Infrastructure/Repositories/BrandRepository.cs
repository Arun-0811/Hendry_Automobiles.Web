using Hendry_Auto.Application.Contracts.Persistence;
using Hendry_Auto.Domain.Models;
using Hendry_Auto.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Hendry_Auto.Infrastructure.Repositories
{
    public class BrandRepository : GenericRepository<Brand>, IBrandRepository
    {
        public BrandRepository(ApplicationDbContext dbcontext) : base(dbcontext)
        {

        }

        public async Task Update(Brand brand)
        {
            var objFromDb = await _dbcontext.Brands.FirstOrDefaultAsync(x => x.Id == brand.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = brand.Name;
                objFromDb.EstablishedYear = brand.EstablishedYear;
                if(brand.BrandLogo !=null)
                {
                    objFromDb.BrandLogo = brand.BrandLogo;
                }
            }
            _dbcontext.Update(objFromDb);
        }
    }
}
