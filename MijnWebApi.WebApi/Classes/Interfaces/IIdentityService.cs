using System.Security.Claims;

namespace MijnWebApi.WebApi.Classes.Interfaces
{
    public interface IIdentityService
    {
        Task<string> GetCurrentUserIdAsync(ClaimsPrincipal user);
    }
}
