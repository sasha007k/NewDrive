using BLL.Services;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewDrive.DTO;
using System;
using System.Diagnostics;
using System.IO;
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

        public IActionResult Index(string? search)
        {
            var k = search;
            var filesFoldersModel = new FolderFilesModel();

            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(search))
            {
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
            }
            else
            {
                filesFoldersModel.Search = search;
                var baseFolder = _folderService.GetBaseFolderByUserId(userId);
                if (baseFolder != null)
                {
                    filesFoldersModel.CurrentFolderId = baseFolder.Id;
                    filesFoldersModel.ParentFolderId = -1;

                    filesFoldersModel.FilesInFolder = _fileService.GetAllFiles().Where(x => x.Name.Contains(search, System.StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }

            if (userId != null)
                CountFullFil(filesFoldersModel);
            return View(filesFoldersModel);
        }

        private void CountFullFil(FolderFilesModel folderFilesModel, int availableMemoryInGb = 20)
        {
            var storagePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Storage");
            var storageSize = ConvertBytesToGigabytes(DirSize(new DirectoryInfo(storagePath)));
            folderFilesModel.FullfilPersentage =   Math.Ceiling((storageSize * 100) / availableMemoryInGb);
            folderFilesModel.FullfilMessage = $"Used memory: {Math.Round(storageSize, 3)} GB of {availableMemoryInGb} GB";
        }

        private double ConvertBytesToGigabytes(long bytes)
        {
            return ((bytes / 1024f) / 1024f) / 1024;
        }

        private long DirSize(DirectoryInfo d)
        {
            long size = 0;
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }

            return size;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Stared()
        {
            var filesFoldersModel = new FolderFilesModel();

            filesFoldersModel.FilesInFolder =  _fileService.GetStared();

            return View(filesFoldersModel);
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
