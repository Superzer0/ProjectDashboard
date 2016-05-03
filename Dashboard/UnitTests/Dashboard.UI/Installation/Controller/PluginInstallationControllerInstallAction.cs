using System;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Dashboard;
using Dashboard.Controllers.API;
using Dashboard.Infrastructure.Identity.Managers;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.Services.Plugins;
using Dashboard.UI.Resources;
using Moq;
using NUnit.Framework;
using UnitTests.Utils;

namespace UnitTests.Dashboard.UI.Installation.Controller
{
    [TestFixture]
    public class PluginInstallationControllerInstallTests : BaseTestFixture
    {
        [Test]
        public async Task FileIdEmpty_BadRequestReturned()
        {
            // Arrange
            var pluginsFacade = AutoMock.Mock<IManagePluginsFacade>();
            var controller = AutoMock.Create<PluginInstallationController>();

            // Act
            var httpActionResult = await controller.InstallPlugin(string.Empty);

            // Assert
            AssertBadRequestMessage(httpActionResult, ExceptionMessages.FileIdInvalid);

            // plugin was not added to queue
            pluginsFacade.Verify(p => p.InstallPluginAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
        }

        [Test]
        public async Task UserNotFound_BadRequestReturned()
        {
            // Arrange
            var applicationUserManager = new Mock<ApplicationUserManager>();
            applicationUserManager.Setup(p => p.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult((DashboardUser)null));

            var pluginsFacade = AutoMock.Mock<IManagePluginsFacade>();

            var controller = AutoMock.Create<PluginInstallationController>();
            controller.UserManager = applicationUserManager.Object;

            // Act
            var httpActionResult = await controller.InstallPlugin("fakeId");

            // Assert
            AssertBadRequestMessage(httpActionResult, ExceptionMessages.UserNotFoundMessage);

            // plugin was not added to queue
            pluginsFacade.Verify(p => p.InstallPluginAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
        }


        [Test]
        public async Task ThrowsInvalidOperationException_BadRequestReturned()
        {
            // Arrange
            var expectedFileId = Guid.NewGuid().ToString();
            var expectedUser = new DashboardUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "user2"
            };

            var applicationUserManager = new Mock<ApplicationUserManager>();
            applicationUserManager.Setup(p => p.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(expectedUser));

            var pluginsFacade = AutoMock.Mock<IManagePluginsFacade>();

            var controller = AutoMock.Create<PluginInstallationController>();
            controller.UserManager = applicationUserManager.Object;

            var expectedException = new InvalidOperationException("sth went wrong");

            pluginsFacade.Setup(p => p.InstallPluginAsync(
                It.Is<string>(r => expectedFileId.Equals(r)),
                It.Is<Guid>(r => r == Guid.Parse(expectedUser.Id)))).Throws(expectedException);

            // Act
            var httpActionResult = await controller.InstallPlugin(expectedFileId);

            // Assert
            AssertBadRequestMessage(httpActionResult, "sth went wrong");


            pluginsFacade.Verify(p => p.InstallPluginAsync(
                It.Is<string>(r => expectedFileId.Equals(r)),
                It.Is<Guid>(r => r == Guid.Parse(expectedUser.Id))), Times.Once);
        }

        [Test]
        public async Task HappyPath()
        {
            // Arrange
            var expectedFileId = Guid.NewGuid().ToString();
            var expectedUser = new DashboardUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "user2"
            };

            var applicationUserManager = new Mock<ApplicationUserManager>();
            applicationUserManager.Setup(p => p.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(expectedUser));

            var pluginsFacade = AutoMock.Mock<IManagePluginsFacade>();

            var controller = AutoMock.Create<PluginInstallationController>();
            controller.UserManager = applicationUserManager.Object;

            pluginsFacade.Setup(p => p.InstallPluginAsync(
                It.Is<string>(r => expectedFileId.Equals(r)),
                It.Is<Guid>(r => r == Guid.Parse(expectedUser.Id))))
                .Returns(Task.CompletedTask);

            // Act
            var httpActionResult = await controller.InstallPlugin(expectedFileId);

            // Assert
            Assert.That(httpActionResult, Is.TypeOf<OkResult>());

            pluginsFacade.Verify(p => p.InstallPluginAsync(
                It.Is<string>(r => expectedFileId.Equals(r)),
                It.Is<Guid>(r => r == Guid.Parse(expectedUser.Id))), Times.Once);
        }

    }
}
