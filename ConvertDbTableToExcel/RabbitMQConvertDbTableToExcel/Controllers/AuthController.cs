using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RabbitMQConvertDbTableToExcel.Models.Dtos;

namespace RabbitMQConvertDbTableToExcel.Controllers
{
    public class AuthController : Controller
    {
        readonly UserManager<IdentityUser> _userManager;
        readonly SignInManager<IdentityUser> _signInManager;
        readonly ILogger<AuthController> _logger;

        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }
        
        [HttpGet]
        public IActionResult Signin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signin(SignInDto signInDto)
        {
            var user = await _userManager.FindByEmailAsync(signInDto.Email);
            if (user != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user, signInDto.Password, signInDto.RememberMe, false);
                return signInResult.Succeeded == true ? RedirectToAction("Index", "Home") : View(signInDto);
            }
            return View(signInDto);
        }

        [HttpGet]
        public async Task<IActionResult> Signout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
