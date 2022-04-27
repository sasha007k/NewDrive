using BLL.Implementations;
using BLL.Services;
using DAL.Models;
using DAL.Repository;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Tests
{
    public class FolderServiceTests
    {
        private IFolderService _folderService;
        private IFolderRepository _folderRepository;
        private IFileRepository _fileRepository;

        [SetUp]
        public void Setup()
        {
            _fileRepository = Substitute.For<IFileRepository>();
            _folderRepository = Substitute.For<IFolderRepository>();
            _folderService = new FolderService(_folderRepository, _fileRepository);
        }

        [Test]
        public async Task GetBaseFolderByUserIdShouldReturnBaseFolder_WhenItExists()
        {
            var folders = new List<Folder> { new Folder() { Id = 1, Name = "file1", OwnerId = "1", ParentFolderId = -1 },
                new Folder() { Id = 2, Name = "file1", OwnerId = "1", ParentFolderId = 1 } };

           var a = _folderRepository.GetAll(x => x.Files, x => x.Folders).ReturnsForAnyArgs(folders.AsQueryable());

            var response = _folderService.GetBaseFolderByUserId("1");

            Assert.IsNotNull(response);
            Assert.IsTrue(1 == response.Id);
        }

        [Test]
        public async Task CreateFolderShouldCreateFolder_WhenFolderIsValid()
        {
            await _folderService.CreateFolder("folder", 1, "1");

            await _folderRepository.ReceivedWithAnyArgs(1)
                .Add(new Folder() { Id = 2, Name = "file1", OwnerId = "1", ParentFolderId = 1 });
        }

        [Test]
        public async Task GetAllFoldersInCurrentFolderShouldReturnAllFoldersInCurrentFolder_WhenTheyExists()
        {
            var folders = new List<Folder> { new Folder() { Id = 1, Name = "file1", OwnerId = "1", ParentFolderId = 1 },
                new Folder() { Id = 2, Name = "file1", OwnerId = "1", ParentFolderId = 1 } };

            _folderRepository.GetAll().Returns(folders.AsQueryable());

            var response = _folderService.GetAllFoldersInCurrentFolder(1);

            Assert.IsNotNull(response);
            Assert.AreEqual(2, response.Count());
        }

        [Test]
        public async Task GetAllFoldersInCurrentFolderShouldReturEmptyList_WhenTheyDoNotExists()
        {
            
            var response = _folderService.GetAllFoldersInCurrentFolder(0);

            Assert.IsNotNull(response);
            Assert.IsEmpty(response);
        }

        [Test]
        public async Task DeleteFolderShouldDeleteFolder_WhenItExists()
        {
            var folder = new List<Folder> { new Folder() { Id = 1, Name = "file1", OwnerId = "1", ParentFolderId = 1 } };

            _folderRepository.GetAll(x => x.Files, x => x.Folders).ReturnsForAnyArgs(folder.AsQueryable());

            await _folderService.DeleteFolder(1);

            await _folderRepository.ReceivedWithAnyArgs(1).Delete(1);
        }
    }
}
