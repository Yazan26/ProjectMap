using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MijnWebApi.WebApi.Classes
{
    public class AccountViewModel
    {
        public IdentityUser User { get; set; }
        public IList<Claim> ClaimsIdentity { get; set; }
    }
}
