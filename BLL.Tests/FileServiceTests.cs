using BLL.Implementations;
using BLL.Services;
using DAL.Repository;
using Microsoft.AspNetCore.Http;
using Moq;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Tests
{
    public class FileServiceTests
    {
        private IFileRepository _fileRepository;
        private IFileService _fileService;

        [SetUp]
        public void Setup()
        {
            _fileRepository = Substitute.For<IFileRepository>();
            _fileService = new FileService(_fileRepository);
        }

        [Test]
        public async Task SaveFileShouldSaveFile_WhenFileIsValid()
        {
            var fileMock = new Mock<IFormFile>();
            var content = "Hello World from a Fake File";
            var fileName = "test.pdf";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var file = fileMock.Object;

            var fileToReturn = new DAL.Models.File { Id = 1, FolderId = 1, Name = "test.pdf", OwnerId = "1", Path = "path" };

            //_fileRepository.Add(fileToReturn).Returns(fileToReturn);

            await _fileService.SaveFile(file, 1, "1");

            await _fileRepository.Received(1).Add(Arg.Any<DAL.Models.File>());
        }

        [Test]
        public async Task SaveFileShouldReturn_WhenFileIsNull()
        {
            await _fileService.SaveFile(null, 1, "1");

            await _fileRepository.DidNotReceive().Add(Arg.Any<DAL.Models.File>());
        }

        [Test]
        public async Task SaveFileShouldReturn_WhenOwnerIdIsNull()
        {

            var fileMock = new Mock<IFormFile>();
            var content = "Hello World from a Fake File";
            var fileName = "test.pdf";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var file = fileMock.Object;

            await _fileService.SaveFile(file, 1, null);

            await _fileRepository.DidNotReceive().Add(Arg.Any<DAL.Models.File>());
        }

        [Test]
        public async Task SaveFileShouldReturn_WhenCurrentFolderIdIsLessThanOne()
        {

            var fileMock = new Mock<IFormFile>();
            var content = "Hello World from a Fake File";
            var fileName = "test.pdf";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var file = fileMock.Object;

            await _fileService.SaveFile(file, 0, "1");

            await _fileRepository.DidNotReceive().Add(Arg.Any<DAL.Models.File>());
        }

        [Test]
        public async Task GetAllFilesInCurrentFolderShouldReturnAllFiles_WhenTheyExists()
        {
            var files = new List<DAL.Models.File> { new DAL.Models.File() { Name = "file1", OwnerId = "1", Path = "path", FolderId = 1 },
                new DAL.Models.File() { Name = "file12", OwnerId = "1", Path = "path1", FolderId = 2 } };
            var returnedFiles = files.AsQueryable();

            _fileRepository.GetAll().Returns(returnedFiles);

            var response = _fileService.GetAllFilesInCurrentFolder(1);

            Assert.IsNotNull(response);
            Assert.AreEqual(1, response.Count());
        }

        [Test]
        public async Task GetAllFilesInCurrentFolderShouldEmptyList_WhenFolderIdIsLessThanOne()
        {
            var response = _fileService.GetAllFilesInCurrentFolder(0);

            Assert.IsNotNull(response);
            Assert.IsEmpty(response);
        }

        [Test]
        public async Task DeleteFileShouldDeleteFile_WhenItExists()
        {
            await _fileService.DeleteFile(1);

            await _fileRepository.Received(1).Delete(Arg.Any<int>());
        }


        [Test]
        public async Task GetAsyncShouldReturnFile_WhenItExists()
        {
            _fileRepository.GetById(1).Returns(new DAL.Models.File { Id = 1, FolderId = 1, Name = "test.pdf", OwnerId = "1", Path = "path" });

            var response = await _fileService.GetAsync(1);

            await _fileRepository.Received(1).GetById(Arg.Any<int>());
            Assert.IsNotNull(response);
            Assert.True(1 == response.Id
                && 1 == response.FolderId
                && "test.pdf" == response.Name
                && "1" == response.OwnerId
                && "path" == response.Path);
        }
    }
}