using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dashboard.Controllers.MVC;
using Dashboard.Infrastructure.ActionResults;
using Dashboard.Infrastructure.Identity.Managers;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.DataObjects.Display;
using Dashboard.UI.Objects.Services;
using Moq;
using NUnit.Framework;
using UnitTests.Utils;

namespace UnitTests.Dashboard.UI.Dispatch.Controller
{
    [TestFixture]
    public class HomeControllerTests : BaseTestFixture
    {
        [Test]
        public async Task Index_HappyPath()
        {
            AutoMock.Mock<IPreparePluginFrontEnd>()
                .Setup(p => p.ProcessActivePluginsHtml(It.IsAny<string>(), It.IsAny<HtmlProcessingOptions>()))
                .Returns(Task.FromResult(new List<ProcessedPluginHtml>() as IEnumerable<ProcessedPluginHtml>));

            AutoMock.Mock<IPreparePluginFrontEnd>()
                .Setup(p => p.ProcessActivePluginsConfiguration(It.IsAny<string>()))
                .Returns(Task.FromResult(new List<ProcessedPluginConfiguration>() as IEnumerable<ProcessedPluginConfiguration>));

            AutoMock.Mock<IPreparePluginFrontEnd>()
                .Setup(p => p.PackPluginHtmlToGrid(It.IsAny<IEnumerable<ProcessedPluginHtml>>(), It.IsAny<IEnumerable<ProcessedPluginConfiguration>>()))
                .Returns(new List<IEnumerable<Tuple<ProcessedPluginHtml, ProcessedPluginConfiguration>>>
                {
                    new List<Tuple<ProcessedPluginHtml, ProcessedPluginConfiguration>>
                    {
                        new Tuple<ProcessedPluginHtml, ProcessedPluginConfiguration>(new ProcessedPluginHtml(), new ProcessedPluginConfiguration())
                    }
                });

            var applicationUserManager = new Mock<ApplicationUserManager>();
            applicationUserManager.Setup(p => p.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new DashboardUser()));

            var adminController = AutoMock.Create<HomeController>();
            adminController.UserManager = applicationUserManager.Object;
            var envMock = new Mock<IEnvironment>();
            adminController.Environment = envMock.Object;

            var actionResult = await adminController.Index();
            Assert.That(actionResult, Is.TypeOf<RazorResult>());
        }

    }
}
