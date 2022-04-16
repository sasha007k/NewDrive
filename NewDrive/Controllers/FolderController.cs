using BLL.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace NewDrive.Controllers
{
    public class FolderController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFolderService _folderService;

        public FolderController(UserManager<IdentityUser> userManager, IFolderService folderService)
        {
            _userManager = userManager;
            _folderService = folderService;
        }

        public async Task<IActionResult> CreateFolder(string folderName, int currentFolderId)
        {
            var userId = _userManager.GetUserId(User);
            await _folderService.CreateFolder(folderName, currentFolderId, userId);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> DeleteFolder(int id)
        {
            await _folderService.DeleteFolder(id);
            return RedirectToAction("Index", "Home");
        }
    }
}
