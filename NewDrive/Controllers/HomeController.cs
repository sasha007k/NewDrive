using BLL.Services;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewDrive.DTO;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NewDrive.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFolderService _folderService;
        private readonly IFileService _fileService;

        public HomeController(UserManager<IdentityUser> userManager, IFolderService folderService, IFileService fileService)
        {
            _userManager = userManager;
            _folderService = folderService;
            _fileService = fileService;
        }

        public IActionResult Index()
        {
            var filesFoldersModel = new FolderFilesModel();

            var userId = _userManager.GetUserId(User);

            if (userId != null)
            {
                var baseFolder = _folderService.GetBaseFolderByUserId(userId);
                if (baseFolder != null)
                {
                    filesFoldersModel.CurrentFolderId = baseFolder.Id;
                    filesFoldersModel.ParentFolderId = -1;

                    filesFoldersModel.FilesInFolder = _fileService.GetAllFilesInCurrentFolder(baseFolder.Id).Where(x => !x.IsDeleted).ToList();
                    filesFoldersModel.FoldersInFolder = _folderService.GetAllFoldersInCurrentFolder(baseFolder.Id).Where(x => !x.IsDeleted).ToList();
                }
            }

            return View(filesFoldersModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult RecycleBin()
        {
            var filesFoldersModel = new FolderFilesModel();

            var userId = _userManager.GetUserId(User);

            if (userId != null)
            {
                var baseFolder = _folderService.GetBaseFolderByUserId(userId);
                if (baseFolder != null)
                {
                    filesFoldersModel.CurrentFolderId = baseFolder.Id;
                    filesFoldersModel.ParentFolderId = -1;

                    filesFoldersModel.FilesInFolder = _fileService.GetAllFilesInCurrentFolder(baseFolder.Id).Where(x => x.IsDeleted).ToList();
                    filesFoldersModel.FoldersInFolder = _folderService.GetAllFoldersInCurrentFolder(baseFolder.Id).Where(x => x.IsDeleted).ToList();
                }
            }

            return View(filesFoldersModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
