using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using System.Web.Http.Results;
using Common.Logging;
using Dashboard;
using Dashboard.Controllers.API;
using Dashboard.Infrastructure.Identity.Managers;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.DataObjects.Validation;
using Dashboard.UI.Objects.Services.Plugins;
using Dashboard.UI.Resources;
using Moq;
using NUnit.Framework;
using UnitTests.Utils;

namespace UnitTests.Dashboard.UI.Installation.Controller
{
    [TestFixture]
    public class PluginInstallationControllerValidateTests : BaseTestFixture
    {
        [Test]
        public async Task FileIdEmpty_BadRequestReturned()
        {
            // Arrange
            var pluginsFacade = AutoMock.Mock<IManagePluginsFacade>();
            var controller = AutoMock.Create<PluginInstallationController>();

            // Act
            var httpActionResult = await controller.ValidatePlugin(string.Empty);

            // Assert
            AssertBadRequestMessage(httpActionResult, ExceptionMessages.FileIdInvalid);

            // plugin was not added to queue
            pluginsFacade.Verify(p => p.ValidatePluginAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
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
            var httpActionResult = await controller.ValidatePlugin("fakeId");

            // Assert
            AssertBadRequestMessage(httpActionResult, ExceptionMessages.UserNotFoundMessage);

            // plugin was not added to queue
            pluginsFacade.Verify(p => p.ValidatePluginAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
        }

        [Test]
        public async Task ValidationResultsNull_InternalServerErrorReturned()
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
            pluginsFacade.Setup(p => p.ValidatePluginAsync(
                It.Is<string>(r => expectedFileId.Equals(r)),
                It.Is<Guid>(r => r == Guid.Parse(expectedUser.Id)))).Returns(
                    Task.FromResult((ConsolidatedPluginValidationResult)null)
                );

            var controller = AutoMock.Create<PluginInstallationController>();
            controller.UserManager = applicationUserManager.Object;

            // Act
            var httpActionResult = await controller.ValidatePlugin(expectedFileId);
            var loggedMessage = DummyLogger.GetLoggedMessages().First();

            // Assert
            Assert.That(httpActionResult, Is.TypeOf<InternalServerErrorResult>());
            Assert.That(loggedMessage.Level, Is.EqualTo(LogLevel.Error));
            Assert.That(loggedMessage.Message.ToString(), Is.Not.Empty);
            Assert.That(loggedMessage.Exception, Is.TypeOf<NullReferenceException>());

            // plugin was not added to queue
            pluginsFacade.Verify(p => p.ValidatePluginAsync(
                It.Is<string>(r => expectedFileId.Equals(r)),
                It.Is<Guid>(r => r == Guid.Parse(expectedUser.Id))), Times.Once);
        }

        [Test]
        public async Task ValidationFailed_BadRequestReturned()
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
            var expectedValidationResults = new ConsolidatedPluginValidationResult
            {
                IsValidated = false,
                PluginValidationResults = new List<PluginValidationResult>
                {
                    new PluginValidationResult
                    {
                        ValidatorName = "dummy.validator",
                        ValidationResults = new List<string> {"some validation message"}
                    }
                }
            };

            pluginsFacade.Setup(p => p.ValidatePluginAsync(
                It.Is<string>(r => expectedFileId.Equals(r)),
                It.Is<Guid>(r => r == Guid.Parse(expectedUser.Id)))).Returns(
                    Task.FromResult(expectedValidationResults)
                );

            var controller = AutoMock.Create<PluginInstallationController>();
            controller.UserManager = applicationUserManager.Object;
            // below required for Request.CreateResponse to work
            controller.Request = new HttpRequestMessage
            {
                Properties = { { HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
            };

            // Act
            var httpActionResult = (ResponseMessageResult)await controller.ValidatePlugin(expectedFileId);

            // Assert
            Assert.That(httpActionResult.Response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var response = await httpActionResult.Response.Content.ReadAsAsync<ConsolidatedPluginValidationResult>();
            Assert.That(expectedValidationResults.Equals(response));
            Assert.That(response, Is.EqualTo(expectedValidationResults));

            // plugin was not added to queue
            pluginsFacade.Verify(p => p.ValidatePluginAsync(
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
            var expectedValidationResults = new ConsolidatedPluginValidationResult
            {
                IsValidated = true,
                PluginValidationResults = new List<PluginValidationResult>
                {
                    new PluginValidationResult
                    {
                        IsSuccess = true,
                        ValidatorName = "dummy.validator",
                        ValidationResults = new List<string> ()
                    }
                }
            };

            pluginsFacade.Setup(p => p.ValidatePluginAsync(
                It.Is<string>(r => expectedFileId.Equals(r)),
                It.Is<Guid>(r => r == Guid.Parse(expectedUser.Id)))).Returns(
                    Task.FromResult(expectedValidationResults)
                );

            var controller = AutoMock.Create<PluginInstallationController>();
            controller.UserManager = applicationUserManager.Object;
            // below required for Request.CreateResponse to work
            controller.Request = new HttpRequestMessage
            {
                Properties = { { HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
            };

            // Act
            var httpActionResult =
                (OkNegotiatedContentResult<ConsolidatedPluginValidationResult>)
                    await controller.ValidatePlugin(expectedFileId);

            // Assert
            var response = httpActionResult.Content;
            Assert.That(expectedValidationResults.Equals(response));
            Assert.That(response, Is.EqualTo(expectedValidationResults));

            // plugin was not added to queue
            pluginsFacade.Verify(p => p.ValidatePluginAsync(
                It.Is<string>(r => expectedFileId.Equals(r)),
                It.Is<Guid>(r => r == Guid.Parse(expectedUser.Id))), Times.Once);
        }
    }
}
