using Hendry_Auto.Application.Contracts.Persistence;
using Hendry_Auto.Infrastructure.Common;
using Hendry_Auto.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpcontextAccessor;
        public UnitOfWork(ApplicationDbContext dbcontext, UserManager<IdentityUser> userManager, IHttpContextAccessor httpcontextAccessor)
        {
            _dbcontext = dbcontext;
            Brand = new BrandRepository(_dbcontext);
            VehicleType = new VehicleTypeRepository(_dbcontext);
            Post = new PostRepository(_dbcontext);
            _userManager = userManager;
            _httpcontextAccessor = httpcontextAccessor;
        }
        public IBrandRepository Brand { get; set; }

        public IVehicleTypeRepository VehicleType { get; private set; }

        public IPostRepository Post { get; private set; }

        public void Dispose()
        {
            _dbcontext.Dispose();
        }

        public async Task SaveAsync()
        {
            _dbcontext.SaveCommonFields(_userManager,_httpcontextAccessor);
            await _dbcontext.SaveChangesAsync();
        }
    }
}
