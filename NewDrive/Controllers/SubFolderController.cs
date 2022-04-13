using BLL.Services;
using Microsoft.AspNetCore.Mvc;
using NewDrive.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewDrive.Controllers
{
    public class SubFolderController : Controller
    {
        private readonly IFolderService _folderService;
        private readonly IFileService _fileService;

        public SubFolderController(IFolderService folderService, IFileService fileService)
        {
            _folderService = folderService;
            _fileService = fileService;
        }

        public IActionResult Index(int id)
        {
            var filesFoldersModel = new FolderFilesModel();
            filesFoldersModel.CurrentFolderId = id;
            filesFoldersModel.FilesInFolder = _fileService.GetAllFilesInCurrentFolder(id);
            filesFoldersModel.FoldersInFolder = _folderService.GetAllFoldersInCurrentFolder(id);
            return View("SubFolder", filesFoldersModel);
        }
    }
}
