using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Dashboard.Controllers.API;
using Dashboard.Infrastructure.Identity.Managers;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.Providers;
using Dashboard.UI.Objects.Services;
using Moq;
using NUnit.Framework;
using UnitTests.Utils;

namespace UnitTests.Dashboard.UI.Dispatch.Controller
{
    [TestFixture]
    public class PluginsControllerTests : BaseTestFixture
    {
        [Test]
        public async Task GetPluginsHappyPath()
        {
            AutoMock.Mock<IProvidePlugins>()
                .Setup(p => p.GetPluginsAsync())
                .Returns(Task.FromResult(new List<Plugin>() as IEnumerable<Plugin>));

            var applicationAccessController = AutoMock.Create<PluginsController>();
            var httpActionResult = await applicationAccessController.InstancePlugins();
            Assert.That(httpActionResult, Is.Not.Null);
        }

        [Test]
        public async Task GetPluginInfo_NotFound()
        {
            AutoMock.Mock<IProvidePlugins>()
                .Setup(p => p.GetPluginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((Plugin)null);

            var applicationUserManager = new Mock<ApplicationUserManager>();
            applicationUserManager.Setup(p => p.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new DashboardUser()));

            var applicationAccessController = AutoMock.Create<PluginsController>();
            applicationAccessController.UserManager = applicationUserManager.Object;

            var httpActionResult = await applicationAccessController.GetPluginInfo("sdf","dsf");
            Assert.That(httpActionResult, Is.TypeOf<NotFoundResult>());
        }


        [Test]
        public async Task GetPluginInfo_HappyPath()
        {
            AutoMock.Mock<IProvidePlugins>()
                .Setup(p => p.GetPluginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new Plugin
                {
                    PluginMethods = new List<PluginMethod>(),
                    Version = "3423",
                    Name = "213",
                    Id = "3423"
                });

            var applicationUserManager = new Mock<ApplicationUserManager>();
            applicationUserManager.Setup(p => p.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new DashboardUser()));

            var applicationAccessController = AutoMock.Create<PluginsController>();
            applicationAccessController.UserManager = applicationUserManager.Object;

            var envMock = new Mock<IEnvironment>();
            applicationAccessController.Environment = envMock.Object;

            var httpActionResult = await applicationAccessController.GetPluginInfo("sdf", "dsf");
            Assert.That(httpActionResult, Is.Not.Null);
        }
    }
}
