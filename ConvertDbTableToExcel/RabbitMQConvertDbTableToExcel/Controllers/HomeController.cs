using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RabbitMQConvertDbTableToExcel.Models;
using System.Diagnostics;

namespace RabbitMQConvertDbTableToExcel.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager, AppDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            if (!_context.Users.Any())
            {
                _userManager.CreateAsync(new IdentityUser
                {
                    UserName = "alomatik",
                    Email = "alomatik@gmail.com",
                }, "kemalDERLER77.").Wait();

                _userManager.CreateAsync(new IdentityUser
                {
                    UserName = "yelimot",
                    Email = "yelimot@gmail.com",
                }, "kemalDERLER77.").Wait();
                _context.SaveChanges();
                _logger.LogInformation("Veritabanına ilk datalar eklendi.");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}