using BLL.Services;
using DAL.Repository;
using Microsoft.AspNetCore.Mvc;
using NewDrive.Controllers;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewDrive.Tests
{
    public class SubFolderControllerTests
    {

        private IFolderService _folderService;
        private IFolderRepository _folderRepository;
        private IFileRepository _fileRepository;
        private IFileService _fileService;
        private SubFolderController _controller;

        [SetUp]
        public void Setup()
        {
            _fileRepository = Substitute.For<IFileRepository>();
            _folderRepository = Substitute.For<IFolderRepository>();
            _folderService = Substitute.For<IFolderService>();
            _fileService = Substitute.For<IFileService>();

            _controller = new SubFolderController(_folderService, _fileService);
        }

        [Test]
        public async Task IndexShouldReturnViewResult_WhenSubfolderExists()
        {
            int id = 1;

            _fileService.GetAllFilesInCurrentFolder(id).ReturnsNull();
            _folderService.GetAllFoldersInCurrentFolder(id).ReturnsNull();

            var response = (ViewResult)_controller.Index(id);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<ViewResult>(response);
        }
    }
}
