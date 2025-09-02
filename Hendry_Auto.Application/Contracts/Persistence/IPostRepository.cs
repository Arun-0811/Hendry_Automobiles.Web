using Hendry_Auto.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hendry_Auto.Application.Contracts.Persistence
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        Task Update(Post post);

        Task<Post> GetPostById(Guid id);

        Task<List<Post>> GetAllPost();

        Task <List<Post>> GetAllPost(Guid? skipRecord, Guid? brandId);

        Task<List<Post>> GetAllPost(string? searchName, Guid? brandId, Guid? VehicleTypeId);
    }
}
