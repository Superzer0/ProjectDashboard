using System;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Dashboard.Controllers.API;
using Dashboard.Infrastructure.Identity.Managers;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.DataObjects.Execution;
using Dashboard.UI.Objects.Services;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnitTests.Utils;

namespace UnitTests.Dashboard.UI.Dispatch.Controller
{
    [TestFixture]
    public class PluginCallDispatchControllerTests : BaseTestFixture
    {
        [Test]
        public async Task RemoteCall_BadModel()
        {
            AutoMock.Mock<ICallRemoteMethods>()
                .Setup(p => p.CallRemoteMethod(It.IsAny<BrokerExecutionInfo>(), It.IsAny<string>()))
                .Returns(Task.FromResult("{'app':'dd'}"));

            var applicationAccessController = AutoMock.Create<PluginCallDispatchController>();
            applicationAccessController.ModelState.AddModelError("sdf","sdf");
            var httpActionResult = await applicationAccessController.RemoteCall("", "", "", new JArray());
            Assert.That(httpActionResult, Is.TypeOf<InvalidModelStateResult>());
        }

        [Test]
        public async Task RemoteCall_ParamsInvalid()
        {
            AutoMock.Mock<ICallRemoteMethods>()
                .Setup(p => p.CallRemoteMethod(It.IsAny<BrokerExecutionInfo>(), It.IsAny<string>()))
                .Returns(Task.FromResult("{'app':'dd'}"));

            var applicationAccessController = AutoMock.Create<PluginCallDispatchController>();
            var httpActionResult = await applicationAccessController.RemoteCall("", "", "", new JArray());
            Assert.That(httpActionResult, Is.TypeOf<BadRequestErrorMessageResult>());
        }

        [Test]
        public async Task RemoteCall_UserNotFound()
        {
            AutoMock.Mock<ICallRemoteMethods>()
                .Setup(p => p.CallRemoteMethod(It.IsAny<BrokerExecutionInfo>(), It.IsAny<string>()))
                .Returns(Task.FromResult("{'app':'dd'}"));

            var applicationUserManager = new Mock<ApplicationUserManager>();
            applicationUserManager.Setup(p => p.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult((DashboardUser)null));

            var applicationAccessController = AutoMock.Create<PluginCallDispatchController>();
            applicationAccessController.UserManager = applicationUserManager.Object;
            var httpActionResult = await applicationAccessController.RemoteCall("sdf", "sdf", "sdf", new JArray());
            Assert.That(httpActionResult, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task RemoteCall_CallException()
        {
            AutoMock.Mock<ICallRemoteMethods>()
                .Setup(p => p.CallRemoteMethod(It.IsAny<BrokerExecutionInfo>(), It.IsAny<string>()))
                .Throws(new ArgumentException());

            var applicationUserManager = new Mock<ApplicationUserManager>();
            applicationUserManager.Setup(p => p.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new DashboardUser()));

            var applicationAccessController = AutoMock.Create<PluginCallDispatchController>();
            applicationAccessController.UserManager = applicationUserManager.Object;
            var httpActionResult = await applicationAccessController.RemoteCall("sdf", "sdf", "sdf", new JArray());
            Assert.That(httpActionResult, Is.TypeOf<BadRequestErrorMessageResult>());
        }

        [Test]
        public async Task RemoteCall_HappyPath()
        {
            AutoMock.Mock<ICallRemoteMethods>()
                .Setup(p => p.CallRemoteMethod(It.IsAny<BrokerExecutionInfo>(), It.IsAny<string>()))
                .Returns(Task.FromResult("{'app':'dd'}"));

            var applicationUserManager = new Mock<ApplicationUserManager>();
            applicationUserManager.Setup(p => p.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new DashboardUser()));

            var applicationAccessController = AutoMock.Create<PluginCallDispatchController>();
            applicationAccessController.UserManager = applicationUserManager.Object;
            var httpActionResult = await applicationAccessController.RemoteCall("sdf", "sdf", "sdf", new JArray());
            Assert.That(httpActionResult, Is.TypeOf<OkNegotiatedContentResult<object>>());
        }
    }
}
