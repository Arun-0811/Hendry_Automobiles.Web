using Hendry_Auto.Application.Services.Interface;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hendry_Auto.Application.Services
{
    public class UserNameService : IUserNameService
    {
        private readonly UserManager<IdentityUser> _userManager;
        public UserNameService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        

        public async Task<string> GetUserName(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return string.Empty;
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                return user.UserName;
            }
            else
            {
                return "N/A";
            }
        }

    }
}
