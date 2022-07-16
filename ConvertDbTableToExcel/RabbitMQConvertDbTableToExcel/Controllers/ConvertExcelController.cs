using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RabbitMQConvertDbTableToExcel.Models;
using RabbitMQConvertDbTableToExcel.Services.RabbitMQServices;

namespace RabbitMQConvertDbTableToExcel.Controllers
{
    [Authorize]
    public class ConvertExcelController : Controller
    {
        readonly AppDbContext _appDbContext;
        readonly UserManager<IdentityUser> _userManager;
        readonly ILogger<ConvertExcelController> _logger;
        readonly RabbitMQPublisher _rabbitMQPublisher;
        public ConvertExcelController(UserManager<IdentityUser> userManager, ILogger<ConvertExcelController> logger, AppDbContext appDbContext, RabbitMQPublisher rabbitMQPublisher)
        {
            _userManager = userManager;
            _logger = logger;
            _appDbContext = appDbContext;
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        public IActionResult Index()
        {
            return View();  
        }

        public async Task<IActionResult> CreateExcel()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            string fileName = $"file-excel-{user.UserName}-{Guid.NewGuid().ToString().Substring(0,3)}";
            UserFile userFile = new UserFile
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                FileName = fileName,
                FileStatus = FileStatus.Creating
            };
            _appDbContext.UserFiles.Add(userFile);
            _appDbContext.SaveChanges();

            _rabbitMQPublisher.Publish(new Shared.CreateExcelMessage { UserFileId = userFile.Id});
            _logger.LogInformation("RabbitMQ'ya mesaj controller'dan gönderildi.");

            TempData["StartCreatingExcel"] = true;

            return RedirectToAction("Files");
        }
        public async Task<IActionResult> Files()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var userFiles = _appDbContext.UserFiles.Where(uf => uf.UserId == user.Id).OrderByDescending(uf=>uf.CreatedDate).ToList();

            return View(userFiles);
        }

    }
}
