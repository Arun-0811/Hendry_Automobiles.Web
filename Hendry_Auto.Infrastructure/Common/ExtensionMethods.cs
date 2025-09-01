using Hendry_Auto.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hendry_Auto.Infrastructure.Common
{
    public static class ExtensionMethods
    {
        public static async Task <string> GetCurrentUserId(UserManager<IdentityUser> _userManager,IHttpContextAccessor _ContextAccessor)
        {
            var userId =  _ContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                var user = await _userManager.GetUserAsync(_ContextAccessor.HttpContext.User);
                userId = user?.Id;
            }
            return userId;
        }
        public static async void SaveCommonFields(this ApplicationDbContext dbcontext, UserManager<IdentityUser> _userManager, IHttpContextAccessor _ContextAccessor)
        {
           var userId =await GetCurrentUserId(_userManager, _ContextAccessor);
            IEnumerable<BaseModel> inSertEntities = dbcontext.ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added)
                .Select(x => x.Entity).OfType<BaseModel>();

            IEnumerable<BaseModel> updateEntities = dbcontext.ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Modified)
                .Select(x => x.Entity).OfType<BaseModel>();

            foreach (var entity in inSertEntities)
            {
                entity.CreatedOn = DateTime.Now;
                entity.CreatedBy = userId;
                entity.ModifiedOn = DateTime.Now;             
            }

            foreach (var entity in updateEntities)
            {
                entity.ModifiedBy = userId;
                entity.ModifiedOn = DateTime.Now;
                
            }
        }
    }
}
