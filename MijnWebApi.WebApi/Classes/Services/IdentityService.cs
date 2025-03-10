using Microsoft.AspNetCore.Identity;
using MijnWebApi.WebApi.Classes.Interfaces;
using System.Security.Claims;

namespace MijnWebApi.WebApi.Classes.Services
{
   public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public IdentityService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> GetCurrentUserIdAsync(ClaimsPrincipal user)
        {
            var identityUser = await _userManager.GetUserAsync(user);
            return identityUser?.Id ?? throw new UnauthorizedAccessException("User not found");
        }
    }
}
