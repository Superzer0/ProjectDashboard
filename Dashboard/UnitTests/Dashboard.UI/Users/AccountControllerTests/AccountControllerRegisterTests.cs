using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Common.Logging;
using Dashboard.Controllers.API;
using Dashboard.Infrastructure.Identity;
using Dashboard.Infrastructure.Identity.Managers;
using Dashboard.Models.Account;
using Dashboard.UI.Objects.Auth;
using Microsoft.AspNet.Identity;
using Moq;
using NUnit.Framework;
using UnitTests.Utils;

namespace UnitTests.Dashboard.UI.Users.AccountControllerTests
{
    [TestFixture]
    public class AccountControllerRegisterTests : BaseTestFixture
    {
        [Test]
        public async Task ModelStateNotValid_BadRequestReturnedWithModelErrors()
        {
            //arrange
            var accountController = AutoMock.Create<AccountController>();
            accountController.ModelState.AddModelError("d", "dd");

            //act
            var register = await accountController.Register(new RegisterViewModel());

            //assert
            Assert.That(register, Is.TypeOf<InvalidModelStateResult>());
        }

        [Test]
        public async Task CreateUserErrors_BadRequestReturnedWithModelErrors()
        {
            //arrange
            var userManagerMock = new Mock<ApplicationUserManager>();
            userManagerMock.Setup(p => p.CreateAsync(It.IsAny<DashboardUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new IdentityResult("some", "errors")));

            var accountController = AutoMock.Create<AccountController>();
            accountController.UserManager = userManagerMock.Object;

            //act
            var register = await accountController.Register(new RegisterViewModel());

            //assert
            Assert.That(register, Is.TypeOf<InvalidModelStateResult>());
        }

        [Test]
        public async Task AddingRoleError_BadRequestReturnedWithModelErrors()
        {
            var userManagerMock = new Mock<ApplicationUserManager>();
            userManagerMock.Setup(p => p.CreateAsync(It.IsAny<DashboardUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success));

            userManagerMock.Setup(p => p.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new IdentityResult("some", "error")));

            var accountController = AutoMock.Create<AccountController>();
            accountController.UserManager = userManagerMock.Object;

            //act
            var register = await accountController.Register(new RegisterViewModel());

            //assert
            Assert.That(register, Is.TypeOf<OkResult>());

            var errorLog = DummyLogger.GetLoggedMessages().Last(p => p.Level == LogLevel.Error);
            var errorMessage = errorLog.Message.ToString();
            Assert.That(errorMessage, Contains.Substring("error"));
            Assert.That(errorMessage, Contains.Substring("some"));
        }

        [Test]
        public async Task HappyPath()
        {
            var expectedRegisterViewModel = new RegisterViewModel
            {
                UserName = "lolek",
                Email = "bolek@lolek.com",
                ConfirmPassword = "hehe",
                Password = "hehe"
            };
            var expectedUserId = Guid.NewGuid().ToString();

            var userManagerMock = new Mock<ApplicationUserManager>();
            userManagerMock.Setup(p => p.CreateAsync(It.IsAny<DashboardUser>(), It.IsAny<string>()))
                .Callback((DashboardUser user, string password) => user.Id = expectedUserId)
                .Returns(Task.FromResult(IdentityResult.Success));

            userManagerMock.Setup(p => p.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success));

            var accountController = AutoMock.Create<AccountController>();
            accountController.UserManager = userManagerMock.Object;

            //act
            var register = await accountController.Register(expectedRegisterViewModel);

            //assert
            Assert.That(register, Is.TypeOf<OkResult>());

            userManagerMock.Verify(
                p =>
                    p.CreateAsync(
                        It.Is<DashboardUser>(
                            r =>
                                r.UserName.Equals(expectedRegisterViewModel.UserName) &&
                                r.Email.Equals(expectedRegisterViewModel.Email)),
                        It.Is<string>(r => r.Equals(expectedRegisterViewModel.Password))), Times.Once);

            userManagerMock.Verify(
                p =>
                    p.AddToRoleAsync(
                        It.Is<string>(userId => expectedUserId.Equals(userId)),
                        It.Is<string>(roleKey => DashboardRoles.User.Equals(roleKey))), Times.Once);

            var infoMessage = DummyLogger.GetLoggedMessages().First(p => p.Level == LogLevel.Info);
            var errorMessage = infoMessage.Message.ToString();
            Assert.That(errorMessage, Contains.Substring(expectedRegisterViewModel.UserName));
            Assert.That(errorMessage, Contains.Substring(expectedRegisterViewModel.Email));
            Assert.That( // check if password is not logged anywhere
                DummyLogger.GetLoggedMessages()
                    .All(r => !r.Message.ToString().Contains(expectedRegisterViewModel.Password)),"Password is logged in register controller");

           
        }
    }
}
