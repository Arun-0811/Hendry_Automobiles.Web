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

        public async Task <List<Post>> GetAllPost(string? SearchName, Guid? brandId, Guid? VehicleTypeId)
        {
            var query = _dbcontext.Posts.Include(x => x.Brand).Include(x => x.VehicleType).OrderByDescending(x=>x.ModifiedOn);
            if (SearchName == string.Empty && brandId == Guid.Empty && VehicleTypeId == Guid.Empty)
            {
                return await query.ToListAsync();
            }
            if (brandId != Guid.Empty)
            {
                query =(IOrderedQueryable<Post>)query.Where(x=>x.BrandId == brandId);
            }
            if (VehicleTypeId != Guid.Empty)
            {
                query = (IOrderedQueryable<Post>)query.Where(x => x.VehicleTypeId == VehicleTypeId);
            }
            if(!string.IsNullOrEmpty(SearchName))
            {
                query = (IOrderedQueryable<Post>)query.Where(x => x.Name.Contains(SearchName));
            }
            return await query.ToListAsync();
        }

        public async Task<List<Post>> GetAllPost(Guid? skipRecord, Guid? brandId)
        {
            var query = _dbcontext.Posts.Include(x => x.Brand).Include(x => x.VehicleType).OrderByDescending(x => x.ModifiedOn);
            if (brandId == Guid.Empty)
            {
                return await query.ToListAsync();
            }

            if(brandId != Guid.Empty)
            {
                query = (IOrderedQueryable<Post>)query.Where(x => x.BrandId == brandId);
            }
            var posts = await query.ToListAsync();
            if (skipRecord.HasValue)
            {
                var recordToRemove = posts.FirstOrDefault(x => x.Id == skipRecord.Value);
                if (recordToRemove != null)
                {
                    posts.Remove(recordToRemove);
                }
            }
            return posts;
        }
    }
}
