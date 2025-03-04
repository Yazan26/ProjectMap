using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MijnWebApi.WebApi.Controllers
{
    [Route("account/[controller]")]
    public class AccountManagementController : Controller
    {
        private readonly ILogger<AccountManagementController> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountManagementController(
            ILogger<AccountManagementController> logger,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.logger = logger;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }
    }
}
