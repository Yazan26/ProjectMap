using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MijnWebApi.WebApi.Classes.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MijnWebApi.WebApi.Controllers
{
    [Route("account/[controller]")]
    [ApiController]
    public class AccountManagementController : Controller
    {
        private readonly ILogger<AccountManagementController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration; // user secrets toevoegen

        public AccountManagementController(
            ILogger<AccountManagementController> logger,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody] RegisterModel model)
        {
          //  _logger.LogInformation("Registratie gestart voor {Email}", model.Email);

            if (!ModelState.IsValid)
            {
             //   _logger.LogWarning("Registratie mislukt: ModelState niet valid.");
                return BadRequest(ModelState);
            }


            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
              //  _logger.LogInformation("Gebruiker succesvol geregistreerd: {Email}", model.Email);
                await _userManager.AddToRoleAsync(user, "User");
                return Ok(new { message = "Registratie succesvol!" });
            }

            foreach (var error in result.Errors)
            {
             //   _logger.LogError("Registratiefout: {ErrorDescription}", error.Description);
                ModelState.AddModelError("Error", error.Description);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] LoginModel model)
        {
           // _logger.LogInformation("Login gestart voor {Email}", model.Email);

            if (!ModelState.IsValid)
            {
            //    _logger.LogWarning("Login mislukt: ModelState niet valid.");
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
           //     _logger.LogWarning("Login geweigerd: Gebruiker niet gevonden {Email}", model.Email);
                return Unauthorized(new { message = "Ongeldige inloggegevens." });
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
             //   _logger.LogWarning("Login geweigerd: Ongeldige gegevens voor {Email}", model.Email);
                return Unauthorized(new { message = "Ongeldige inloggegevens." });
            }

           // _logger.LogInformation("Login succesvol voor {Email}", model.Email);

            //  Call the method to generate a token \\
            var token = await GenerateJwtToken(user);

            return Ok(new { token }); //  Return token directly instead of wrapping it in "result"
        }

        private async Task<string> GenerateJwtToken(IdentityUser user)
        {
            var jwtSecret = _configuration["JwtSecret"];
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecret);

            var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id),
    new Claim(ClaimTypes.Email, user.Email)
};

            //  Ensure correct role claim format for ASP.NET Core
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role)); // Use "role" here instead of ClaimTypes.Role
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


    }
}
