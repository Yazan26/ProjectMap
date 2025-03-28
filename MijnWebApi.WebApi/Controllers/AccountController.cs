//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
//namespace MijnWebApi.WebApi.Controllers
//{
//    public class AccountController : Controller
//    {
//        [Authorize]
//        [HttpGet("me")]
//        public IActionResult GetCurrentUser()
//        {
//            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

//            if (string.IsNullOrEmpty(userId))
//            {
//                return Unauthorized("❌ Kan User ID niet ophalen!");
//            }

//            return Ok(new { UserId = userId });
//        }
//    }
//}
