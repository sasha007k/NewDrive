using BLL.Models;
using BLL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NewDrive.Controllers;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NewDrive.Tests
{
    public class FileControllerTests
    {
        private static List<IdentityUser> _users = new List<IdentityUser>() { new IdentityUser()
            {
                Id = "1",
                UserName = "username",
                Email = "username@gmail.com",
                PasswordHash = "123456767889dsaOsaf",
                PhoneNumber = "+380685555555",
                TwoFactorEnabled = false,
                AccessFailedCount = 0,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            } };

        private UserManager<IdentityUser> _userManager = MockUserManager<IdentityUser>(_users).Object;
        private ILogger<FileController> _logger;
        private IFileService _fileService;
        private IBackgroundQueue<Notification> _queue;
        private FileController _controller;

        [SetUp]
        public void Setup()
        {

            _logger = Substitute.For<ILogger<FileController>>();
            _fileService = Substitute.For<IFileService>();
            _queue = Substitute.For<IBackgroundQueue<Notification>>();
            _controller = new FileController(_userManager, _logger, _fileService, _queue);
        }

        [Test]
        public async Task UploadFileShouldUploadFile_WhenItIsValid()
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

            _userManager.GetUserId(new ClaimsPrincipal());

            RedirectToActionResult redirectResult = (RedirectToActionResult) await _controller.UploadFile(file, 1);

            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task DownloadFileShouldDownloadFile_WhenItExists()
        {
            _fileService.GetFile("1").Returns((new byte[] { 0x20 }, "1"));

            var response =(FileContentResult) await _controller.DownloadFile("1");

            Assert.IsNotNull(response);
            Assert.True(response.FileDownloadName == "1");
        }

        [Test]
        public async Task UpdateFileShouldUpdateFile_WhenItExists()
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

            var response = await _controller.UpdateFile(file, "path");
            
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NoContentResult>(response);
        }

        public static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("1");

            return mgr;
        }
    }
}