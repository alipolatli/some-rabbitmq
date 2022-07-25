using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RabbitMQConvertDbTableToExcel.Hubs;
using RabbitMQConvertDbTableToExcel.Models;

namespace RabbitMQConvertDbTableToExcel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelFilesController : ControllerBase
    {
        IHubContext<MessageHub> _hubContext; 
        readonly AppDbContext _appDbContext;
        readonly ILogger<ExcelFilesController> _logger;

        public ExcelFilesController(AppDbContext appDbContext, ILogger<ExcelFilesController> logger, IHubContext<MessageHub> hubContext)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile excelFile,string userFileId)
        {
            if (excelFile.Length < 0) return BadRequest();
            var userFile = await _appDbContext.UserFiles.FirstAsync(uf => uf.Id == userFileId);
            var userFileNameWithExtension= userFile.FileName + Path.GetExtension(excelFile.FileName);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", userFileNameWithExtension);

            await using (FileStream fileStream = new FileStream(filePath,FileMode.Create))
            {
                excelFile.CopyTo(fileStream);
            }
            userFile.CreatedDate = DateTime.Now;
            userFile.FilePath = userFileNameWithExtension;
            userFile.FileStatus = FileStatus.Competed;

            await _appDbContext.SaveChangesAsync();
            //SignalR notification...

            await _hubContext.Clients.User(userFile.UserId).SendAsync("ReceiveInfoMessage");
            return Ok();
        }
    }
}
