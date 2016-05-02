using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Autofac.Extras.Moq;
using Common.Logging;
using Dashboard;
using Dashboard.Controllers.API;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.Providers;
using Dashboard.UI.Objects.Services.Plugins;
using Dashboard.UI.Resources;
using Moq;
using NUnit.Framework;
using UnitTests.Utils;

namespace UnitTests.Dashboard.UI.Installation.Controller
{
    [TestFixture]
    public class PluginInstallationControllerUploadPluginTests
    {
        private AutoMock AutoMock { get; set; }
        private InMemoryLogger DummyLogger { get; set; }

        [SetUp]
        public void SetUp()
        {
            AutoMock = AutoMock.GetLoose();
            DummyLogger = InMemoryLoggingAdapterFactory.CreateDummyLogger();
            LogManager.Adapter = new InMemoryLoggingAdapterFactory(DummyLogger);
        }

        [TearDown]
        public void TearDown()
        {
            AutoMock.Dispose();
        }

        [Test]
        public async Task IsNotMultiPartContent_ThrowsHttpException()
        {
            AutoMock.MockRepository.Create<IProvideFiles>()
                .Setup(p => p.ValidateRequest(It.IsAny<HttpRequestMessage>()))
                .Returns(false);

            var controller = AutoMock.Create<PluginInstallationController>();

            try
            {
                await controller.UploadPlugin();
            }
            catch (HttpResponseException e)
            {
                Assert.That(e.Response.StatusCode == HttpStatusCode.UnsupportedMediaType);
                return;
            }

            Assert.Fail("Exception was not thrown");
        }

        [Test]
        public async Task UserNotFound_FileNotReceivedBadRequestReturned()
        {
            // Arrange
            var applicationUserManager = new Mock<ApplicationUserManager>();
            applicationUserManager.Setup(p => p.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult((DashboardUser)null));

            AutoMock.Mock<IProvideFiles>()
                .Setup(p => p.ValidateRequest(It.IsAny<HttpRequestMessage>()))
                .Returns(true);

            var pluginsFacade = AutoMock.Mock<IManagePluginsFacade>();

            var controller = AutoMock.Create<PluginInstallationController>();
            controller.UserManager = applicationUserManager.Object;

            // Act

            var httpActionResult = await controller.UploadPlugin();
            // Assert
            AssertBadRequestMessage(httpActionResult, ExceptionMessages.UserNotFoundMessage);

            // plugin was not added to queue
            pluginsFacade.Verify(p => p.AddToValidationQueue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
        }

        [Test]
        public async Task FileEmpty_BadRequestReturned()
        {
            var applicationUserManager = new Mock<ApplicationUserManager>();
            applicationUserManager.Setup(p => p.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new DashboardUser()));

            AutoMock.Mock<IProvideFiles>()
                .Setup(p => p.ValidateRequest(It.IsAny<HttpRequestMessage>()))
                .Returns(true);

            AutoMock.Mock<IProvideFiles>()
                .Setup(p => p.ReceiveFile(It.IsAny<HttpRequestMessage>()))
                .Returns(Task.FromResult((UploadedFileMetadata)null));

            var pluginsFacade = AutoMock.Mock<IManagePluginsFacade>();

            var controller = AutoMock.Create<PluginInstallationController>();
            controller.UserManager = applicationUserManager.Object;

            var httpActionResult = await controller.UploadPlugin();
            AssertBadRequestMessage(httpActionResult, ExceptionMessages.FilenNotFoundMessage);

            pluginsFacade.Verify(p => p.AddToValidationQueue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
        }

        [Test]
        public async Task FileNotZip_BadRequestReturned()
        {
            var applicationUserManager = new Mock<ApplicationUserManager>();
            applicationUserManager.Setup(p => p.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new DashboardUser()));

            AutoMock.Mock<IProvideFiles>()
                .Setup(p => p.ValidateRequest(It.IsAny<HttpRequestMessage>()))
                .Returns(true);

            AutoMock.Mock<IProvideFiles>()
                .Setup(p => p.ReceiveFile(It.IsAny<HttpRequestMessage>()))
                .Returns(Task.FromResult(new UploadedFileMetadata()));

            var pluginsFacade = AutoMock.Mock<IManagePluginsFacade>();

            var controller = AutoMock.Create<PluginInstallationController>();
            controller.UserManager = applicationUserManager.Object;

            var httpActionResult = await controller.UploadPlugin();
            AssertBadRequestMessage(httpActionResult, ExceptionMessages.FileNotZipMessage);

            pluginsFacade.Verify(p => p.AddToValidationQueue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
        }

        [Test]
        public async Task UnexpectedExceptionThrown_ErrorLoggedInternalServerErrorReturned()
        {
            // Arrange
            var applicationUserManager = new Mock<ApplicationUserManager>();
            var thrownException = new ArgumentException("My Exception");
            applicationUserManager.Setup(p => p.FindByNameAsync(It.IsAny<string>()))
                .Throws(thrownException);

            AutoMock.Mock<IProvideFiles>()
                .Setup(p => p.ValidateRequest(It.IsAny<HttpRequestMessage>()))
                .Returns(true);

            var pluginsFacade = AutoMock.Mock<IManagePluginsFacade>();

            var controller = AutoMock.Create<PluginInstallationController>();
            controller.UserManager = applicationUserManager.Object;

            //Act
            var httpActionResult = await controller.UploadPlugin();

            // Assert
            Assert.That(httpActionResult, Is.TypeOf<InternalServerErrorResult>());
            var loggedMessage = DummyLogger.GetLoggedMessages().First();
            Assert.That(loggedMessage.Level, Is.EqualTo(LogLevel.Error));
            Assert.That(loggedMessage.Message.ToString(), Is.EqualTo("Unexpected error"));
            Assert.That(loggedMessage.Exception, Is.SameAs(thrownException));

            pluginsFacade.Verify(p => p.AddToValidationQueue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
        }

        [Test]
        public async Task HappyPath_FileAddedToValidationQueue_FileIdReturned()
        {
            // arrange
            var userGuid = Guid.NewGuid().ToString();
            var fileId = $"{Guid.NewGuid()}.zip";

            var applicationUserManager = new Mock<ApplicationUserManager>();
            applicationUserManager.Setup(p => p.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new DashboardUser
                {
                    UserName = "test1",
                    Id = userGuid
                }));

            AutoMock.Mock<IProvideFiles>()
                .Setup(p => p.ValidateRequest(It.IsAny<HttpRequestMessage>()))
                .Returns(true);

            var uploadedFileMetadata = new UploadedFileMetadata
            {
                LocalFileName = $"path/to/file/{fileId}",
                ReceivedFileName = "test.zip"
            };

            AutoMock.Mock<IProvideFiles>()
                .Setup(p => p.ReceiveFile(It.IsAny<HttpRequestMessage>()))
                .Returns(Task.FromResult(uploadedFileMetadata));

            var pluginsFacade = AutoMock.Mock<IManagePluginsFacade>();

            var controller = AutoMock.Create<PluginInstallationController>();
            controller.UserManager = applicationUserManager.Object;

            // act
            var httpActionResult = await controller.UploadPlugin();

            // assert
            //Assert.That(httpActionResult, Is.TypeOf<OkNegotiatedContentResult<dynamic>>());
            var okResponse = (OkNegotiatedContentResult<Dictionary<string, string>>)httpActionResult;
            Assert.That(okResponse.Content["fileId"], Is.EqualTo(fileId));

            pluginsFacade.Verify(p => p.AddToValidationQueue(
                It.Is<string>(v => fileId.Equals(v)),
                It.Is<string>(v => uploadedFileMetadata.LocalFileName.Equals(v)),
                It.Is<Guid>(v => v == Guid.Parse(userGuid))), Times.Once);

        }

        private void AssertBadRequestMessage(IHttpActionResult httpActionResult, string message)
        {
            Assert.That(httpActionResult, Is.TypeOf<BadRequestErrorMessageResult>());
            var badActionResult = (BadRequestErrorMessageResult)httpActionResult;
            Assert.That(badActionResult.Message, Is.EqualTo(message));
        }

    }
}
