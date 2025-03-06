using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MijnWebApi.WebApi.Classes;
using MijnWebApi.WebApi.Classes.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MijnWebApi.WebApi.Controllers
{
    [Route("account/[controller]")]
    [Authorize(Policy = "Level8WizardOnly")]
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

        [HttpPost("{id}/{claim}/{value?}", Name = "AddClaimToUser")]
        public async Task<ActionResult> AddWizardClaimToUser(
            string id, string claim, string value = null!)
        {
            if (id is null)
            {
                return BadRequest("Id is required");
            }
            var user = await userManager.FindByIdAsync(id);
            if (user is null)
            {
                return NotFound("User not found");
            }
            var addClaimsResult = await userManager.AddClaimAsync(user, new Claim(claim, value));

            if (!addClaimsResult.Succeeded)
            {
                return BadRequest();
            }
            return Ok(new AccountViewModel()
            {
                User = user,
                ClaimsIdentity = userManager.GetClaimsAsync(user).Result
            });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return Ok();
            }

            if (result.IsLockedOut)
            {
                return Forbid();
            }

            return Unauthorized();
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok();
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }
    }
}