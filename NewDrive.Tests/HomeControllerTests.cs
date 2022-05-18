using BLL.Services;
using DAL.Models;
using DAL.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewDrive.Controllers;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NewDrive.Tests
{
    public class HomeControllerTests
    {
        private IFolderService _folderService;
        private IFolderRepository _folderRepository;
        private IFileRepository _fileRepository;
        private IFileService _fileService;
        private HomeController _controller;

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

        [SetUp]
        public void Setup()
        {
            _fileRepository = Substitute.For<IFileRepository>();
            _folderRepository = Substitute.For<IFolderRepository>();
            _folderService = Substitute.For<IFolderService>();
            _fileService = Substitute.For<IFileService>();

            _controller = new HomeController(_userManager, _folderService, _fileService);
        }

        [Test]
        public async Task IndexShouldReturnView_WhenUserIdIsNotNull()
        {
            _userManager.GetUserId(new ClaimsPrincipal());

            _folderService.GetBaseFolderByUserId("1").Returns(new Folder() { Id = 1, Name = "file1", OwnerId = "1", ParentFolderId = -1 });

            var response = _controller.Index("");

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<ViewResult>(response);
        }

        [Test]
        public async Task PrivacyShouldReturnViewResult_WhenEverythingIsUkraine()
        {
            var response = _controller.Privacy();

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<ViewResult>(response);
        }

        [Test]
        public async Task ErrorShouldReturnViewResult_WhenEverythingIsBad()
        {
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Request.Headers["device-id"] = "20317";

            var response = _controller.Error();

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<ViewResult>(response);
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
