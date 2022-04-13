  using BLL.Models;
using BLL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace NewDrive.Controllers
{
    public class FileController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<FileController> _logger;
        private readonly IFileService _fileService;
        private readonly IBackgroundQueue<Notification> _queue;

        public FileController(UserManager<IdentityUser> userManager, ILogger<FileController> logger, IFileService fileService,
            IBackgroundQueue<Notification> queue)
        {
            _userManager = userManager;
            _logger = logger;
            _fileService = fileService;
            _queue = queue;
        }

        public async Task<IActionResult> UploadFile(IFormFile file, int currentFolderId)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                await _fileService.SaveFile(file, currentFolderId, userId);
                var notification = new Notification { Message = $"NO WORRIES. {file.Name} is uploading." };
                _queue.Enqueue(notification);
                return RedirectToAction("Index", "Home");
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                throw e;
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(string path)
        {
            var res = _fileService.GetFile(path);

            return File(res.Result.Item1, "application/octet-stream", res.Result.Item2);
        }

        public async Task<IActionResult> DeleteFile(int id)
        {
            await _fileService.DeleteFile(id);
            return NoContent();
        }

        public async Task<IActionResult> UpdateFile(IFormFile file, string path)
        {
            await _fileService.UpdateFile(file, path);
            return NoContent();
        }
    }
}
