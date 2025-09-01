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
    public class PostRepository : GenericRepository<Post>,IPostRepository
    {
        public PostRepository(ApplicationDbContext dbcontext) : base(dbcontext)
        {
        }

        public async Task<List<Post>> GetAllPost()
        {
            return await _dbcontext.Posts.Include(x => x.Brand).Include(x => x.VehicleType).ToListAsync();
        }

        

        public Task<Post> GetPostById(Guid id)
        {
            return _dbcontext.Posts.Include(x => x.Brand).Include(x => x.VehicleType).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Update(Post post)
        {
            var ObjFromDb = await _dbcontext.Posts.FirstOrDefaultAsync(x => x.Id == post.Id);

            if (ObjFromDb != null)
            {
                ObjFromDb.BrandId = post.BrandId;
                ObjFromDb.VehicleTypeId = post.VehicleTypeId;
                ObjFromDb.Name = post.Name;
                ObjFromDb.EngineAndFuelType = post.EngineAndFuelType;
                ObjFromDb.Transmission = post.Transmission;
                ObjFromDb.Engine = post.Engine;
                ObjFromDb.Range = post.Range;
                ObjFromDb.Ratings = post.Ratings;
                ObjFromDb.PriceTo = post.PriceTo;
                ObjFromDb.PriceFrom = post.PriceFrom;
                ObjFromDb.SeatingCapacity = post.SeatingCapacity;
                ObjFromDb.TopSpeed = post.TopSpeed;
                ObjFromDb.Mileage = post.Mileage;

                if (post.VehicleImage != null)
                {
                    ObjFromDb.VehicleImage = post.VehicleImage;
                }

                _dbcontext.Posts.Update(ObjFromDb);   // ✅ safe inside the check
            }
            else
            {
                throw new InvalidOperationException($"Post with Id {post.Id} not found.");
            }
        }

    }
}
