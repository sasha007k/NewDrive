using BLL.Services;
using DAL.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewDrive.Controllers;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NewDrive.Tests
{
    public class FolderControllerTests
    {
        private IFolderService _folderService;
        private IFolderRepository _folderRepository;
        private IFileRepository _fileRepository;
        private FolderController _controller;

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
            _controller = new FolderController(_userManager, _folderService);
        }

        [Test]
        public async Task CreateFolderShouldReturnRedirectToActionResult_WhenItsCreated()
        {
            _userManager.GetUserId(new ClaimsPrincipal());

            RedirectToActionResult response = (RedirectToActionResult)await _controller.CreateFolder("folder", 1);

            Assert.That(response.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task DeleteFolderShouldReturnRedirectToActionResult_WhenItsDeleted()
        {

            RedirectToActionResult response = (RedirectToActionResult)await _controller.DeleteFolder(1);

            Assert.That(response.ActionName, Is.EqualTo("Index"));
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
