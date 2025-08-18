using Hendry_Auto.Application.Contracts.Persistence;
using Hendry_Auto.Domain.Common;
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
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseModel
    {
        protected readonly ApplicationDbContext _dbcontext;

        public GenericRepository(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        
        public async Task Create(T entity)
        {
            await _dbcontext.AddAsync(entity);
        }

        public async Task Delete(T entity)
        {
            _dbcontext.Remove(entity);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task<List<T>> Get(Expression<Func<T, bool>> Predicate)
        {
           return await _dbcontext.Set<T>().Where(Predicate).ToListAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
           return await _dbcontext.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbcontext.Set<T>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<bool> IsRecordExists(Expression<Func<T, bool>> predicate)
        {
            return _dbcontext.Set<T>().Where(predicate).AnyAsync(predicate);
        }

        public IEnumerable<T> Query(Expression<Func<T, bool>> Predicate)
        {
            var entities = _dbcontext.Set<T>().Where(Predicate).ToList();
            return entities;
        }

        public IEnumerable<T> Query()
        {
            var entities = _dbcontext.Set<T>().AsNoTracking().ToList();
            return entities;
        }
    }
}
