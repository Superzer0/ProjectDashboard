using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Dashboard.Controllers.API;
using Dashboard.Models.Account;
using Dashboard.Models.ApplicationAccess;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.Services;
using Moq;
using NUnit.Framework;
using UnitTests.Utils;

namespace UnitTests.Dashboard.UI.Dispatch.Controller
{
    [TestFixture]
    public class ApplicationAccessControllerTests : BaseTestFixture
    {
        [Test]
        public void GetRefreshToken()
        {
            AutoMock.Mock<IAuthRepository>().Setup(p => p.GetAllRefreshTokens()).Returns(new List<AuthRefreshToken>());
            var applicationAccessController = AutoMock.Create<ApplicationAccessController>();
            var httpActionResult = applicationAccessController.GetRefreshTokens();
            Assert.That(httpActionResult, Is.TypeOf<OkNegotiatedContentResult<IEnumerable<AuthRefreshToken>>>());
        }

        [Test]
        public async Task DeleteRefreshToken_Ok()
        {
            AutoMock.Mock<IAuthRepository>().Setup(p => p.RemoveRefreshToken(It.IsAny<string>())).Returns(Task.FromResult(true));
            var applicationAccessController = AutoMock.Create<ApplicationAccessController>();
            var httpActionResult = await applicationAccessController.Delete("sdfsdf");
            Assert.That(httpActionResult, Is.TypeOf<OkResult>());
        }

        [Test]
        public async Task DeleteRefreshToken_Bad()
        {
            AutoMock.Mock<IAuthRepository>().Setup(p => p.RemoveRefreshToken(It.IsAny<string>())).Returns(Task.FromResult(false));
            var applicationAccessController = AutoMock.Create<ApplicationAccessController>();
            var httpActionResult = await applicationAccessController.Delete("sdfsdf");
            Assert.That(httpActionResult, Is.TypeOf<BadRequestErrorMessageResult>());
        }


        [Test]
        public async Task DeleteAppn_Ok()
        {
            AutoMock.Mock<IAuthRepository>().Setup(p => p.DeleteClient(It.IsAny<string>())).Returns(Task.FromResult(true));
            var applicationAccessController = AutoMock.Create<ApplicationAccessController>();
            var httpActionResult = await applicationAccessController.DeleteApp("sdfsdf");
            Assert.That(httpActionResult, Is.TypeOf<OkResult>());
        }


        [Test]
        public async Task DeleteApp_Bad()
        {
            AutoMock.Mock<IAuthRepository>().Setup(p => p.DeleteClient(It.IsAny<string>())).Returns(Task.FromResult(false));
            var applicationAccessController = AutoMock.Create<ApplicationAccessController>();
            var httpActionResult = await applicationAccessController.DeleteApp("sdfsdf");
            Assert.That(httpActionResult, Is.TypeOf<BadRequestResult>());
        }


        [Test]
        public async Task DeactivateApp_Ok()
        {
            AutoMock.Mock<IAuthRepository>().Setup(p => p.ToggleAppState(It.IsAny<string>(), It.IsAny<bool>())).Returns(Task.FromResult(true));
            var applicationAccessController = AutoMock.Create<ApplicationAccessController>();
            var httpActionResult = await applicationAccessController.DeactivateApp("sdfsdf", false);
            Assert.That(httpActionResult, Is.TypeOf<OkResult>());
        }


        [Test]
        public async Task DeactivateApp_Bad()
        {
            AutoMock.Mock<IAuthRepository>().Setup(p => p.ToggleAppState(It.IsAny<string>(), It.IsAny<bool>())).Returns(Task.FromResult(false));
            var applicationAccessController = AutoMock.Create<ApplicationAccessController>();
            var httpActionResult = await applicationAccessController.DeactivateApp("sdfsdf", false);
            Assert.That(httpActionResult, Is.TypeOf<BadRequestResult>());
        }

        [Test]
        public async Task RegenerateAppSecret_Ok()
        {
            AutoMock.Mock<IAuthRepository>().Setup(p => p.ReGenerateAppSecret(It.IsAny<string>())).Returns(Task.FromResult("secret"));
            var applicationAccessController = AutoMock.Create<ApplicationAccessController>();
            var httpActionResult = await applicationAccessController.RegenerateAppSecret("sdfsdf");
            Assert.That(httpActionResult, Is.TypeOf<OkNegotiatedContentResult<string>>());
        }


        [Test]
        public async Task RegenerateAppSecret_Bad()
        {
            AutoMock.Mock<IAuthRepository>()
                .Setup(p => p.ReGenerateAppSecret(It.IsAny<string>()))
                .Throws(new ArgumentException());

            var applicationAccessController = AutoMock.Create<ApplicationAccessController>();
            var httpActionResult = await applicationAccessController.RegenerateAppSecret("sdfsdf");
            Assert.That(httpActionResult, Is.TypeOf<BadRequestErrorMessageResult>());
        }

        [Test]
        public void GetApps_HappyPath()
        {
            AutoMock.Mock<IAuthRepository>()
                .Setup(p => p.GetAllClients())
                .Returns(new List<AuthClient>());

            AutoMock.Mock<IAuthRepository>()
                .Setup(p => p.GetAllRefreshTokens())
                .Returns(new List<AuthRefreshToken>());


            var applicationAccessController = AutoMock.Create<ApplicationAccessController>();
            var httpActionResult = applicationAccessController.GetApps();
            Assert.That(httpActionResult, Is.TypeOf<OkNegotiatedContentResult<IEnumerable<RegisteredClientApplications>>>());
        }

        [Test]
        public void CreateApp_HappyPath()
        {
            AutoMock.Mock<IAuthRepository>()
                .Setup(p => p.CreateClient(It.IsAny<string>(), It.IsAny<AuthApplicationType>(), It.IsAny<string>()))
                .Returns(new AuthClient());

            var applicationAccessController = AutoMock.Create<ApplicationAccessController>();
            var httpActionResult = applicationAccessController.CreateApp(new CreateAppViewModel());
            Assert.That(httpActionResult, Is.TypeOf<OkNegotiatedContentResult<AuthClient>>());
        }

        [Test]
        public void CreateApp_InvalidViewModel()
        {
            AutoMock.Mock<IAuthRepository>()
                .Setup(p => p.CreateClient(It.IsAny<string>(), It.IsAny<AuthApplicationType>(), It.IsAny<string>()))
                .Returns(new AuthClient());

            var applicationAccessController = AutoMock.Create<ApplicationAccessController>();
            applicationAccessController.ModelState.AddModelError("sdf", "sdf");
            var httpActionResult = applicationAccessController.CreateApp(new CreateAppViewModel());
            Assert.That(httpActionResult, Is.TypeOf<InvalidModelStateResult>());
        }

        [Test]
        public void CreateApp_ArgumentException()
        {
            AutoMock.Mock<IAuthRepository>()
                .Setup(p => p.CreateClient(It.IsAny<string>(), It.IsAny<AuthApplicationType>(), It.IsAny<string>()))
                .Throws(new ArgumentException());

            var applicationAccessController = AutoMock.Create<ApplicationAccessController>();
            var httpActionResult = applicationAccessController.CreateApp(new CreateAppViewModel());
            Assert.That(httpActionResult, Is.TypeOf<BadRequestErrorMessageResult>());
        }
    }
}
