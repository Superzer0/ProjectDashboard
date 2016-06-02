using System.Threading.Tasks;
using Dashboard.Services.Display;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Display;
using Dashboard.UI.Objects.Providers;
using Moq;
using NUnit.Framework;
using UnitTests.Utils;
using System.Collections.Generic;
using Dashboard.UI.Objects.DataObjects.Extract;

namespace UnitTests.Dashboard.UI.Front
{
    [TestFixture]
    public class PluginFrontPreprocessorTests : BaseTestFixture
    {
        [Test]
        public async Task ProcessPluginHtmlHappyPath()
        {
            AutoMock.Mock<IProvidePlugins>()
                .Setup(p => p.GetPluginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new Plugin
                {
                    Id = "sdf",
                    Name = "sdfsdf",
                    Version = "fsdfasdf",
                });

            AutoMock.Mock<IManagePluginsStorage>()
                .Setup(p => p.GetPluginIndexFile(It.IsAny<Plugin>()))
                .Returns(Task.FromResult("<span dd> sdf </span>"));

            var pluginFrontPreprocessor = AutoMock.Create<PluginFrontPreprocessor>();

            var processedPluginHtml = await pluginFrontPreprocessor.ProcessPluginHtml("sdf", "sdf", new HtmlProcessingOptions()
            {
                ApiAppIdTag = "dd",
                BaseAddress = "http://www.onet.pl",
                ResourcePrefixTag = "db"
            });

            Assert.IsNotNull(processedPluginHtml);
        }


        [Test]
        public async Task ProcessPluginsHtmlHappyPath()
        {
            AutoMock.Mock<IProvidePlugins>()
                .Setup(p => p.GetActiveUserPluginsAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new List<Plugin>
                {
                    new Plugin
                    {
                        Id = "sdf",
                        Name = "sdfsdf",
                        Version = "fsdfasdf"
                    }
                } as IEnumerable<Plugin>));


            AutoMock.Mock<IManagePluginsStorage>()
                .Setup(p => p.GetPluginIndexFile(It.IsAny<Plugin>()))
                .Returns(Task.FromResult("<span dd> sdf </span>"));

            var pluginFrontPreprocessor = AutoMock.Create<PluginFrontPreprocessor>();

            var processedPluginHtml = await pluginFrontPreprocessor.ProcessActivePluginsHtml("sdf", new HtmlProcessingOptions()
            {
                ApiAppIdTag = "dd",
                BaseAddress = "http://www.onet.pl",
                ResourcePrefixTag = "db"
            });

            Assert.IsNotNull(processedPluginHtml);
        }

        [Test]
        public async Task ProcessActivePluginsConfigurationHappyPath()
        {
            AutoMock.Mock<IProvidePlugins>()
                .Setup(p => p.GetActiveUserPluginsConfiguration(It.IsAny<string>()))
                .Returns(Task.FromResult(new List<PluginUiConfiguration>
                {
                    new PluginUiConfiguration
                    {
                        JsonConfiguration = "{}",
                        Id = "sdfsd",
                        Version = "sdfsdf",
                        Disabled = false,
                        UserId = "fsdfsdsd"
                    }
                } as IEnumerable<PluginUiConfiguration>));


            AutoMock.Mock<IManagePluginsStorage>()
                .Setup(p => p.GetPluginIndexFile(It.IsAny<Plugin>()))
                .Returns(Task.FromResult("<span dd> sdf </span>"));

            var pluginFrontPreprocessor = AutoMock.Create<PluginFrontPreprocessor>();

            var processedConfiguration = await pluginFrontPreprocessor.ProcessActivePluginsConfiguration("sdf");

            Assert.IsNotNull(processedConfiguration);
        }

        [Test]
        public async Task PackPluginHtmlToGridHappyPath()
        {
            IEnumerable<ProcessedPluginHtml> processedPlugins = new List<ProcessedPluginHtml>
            {
                new ProcessedPluginHtml
                {
                    Plugin = new Plugin()
                    {
                        Id = "aaa",
                        Version = "bbb"
                    },
                    Content = "ddddd"
                }
            };

            IEnumerable<ProcessedPluginConfiguration> pluginConfiguration = new List<ProcessedPluginConfiguration>
            {
                new ProcessedPluginConfiguration
                {
                    PluginUniqueId = "aaa-bbb",
                    DisplaySettings = new PluginDisplaySettings(),
                    JsonConfiguration = "sdfds",
                    DispatchLink = "aaaa"
                }
            };

            var pluginFrontPreprocessor = AutoMock.Create<PluginFrontPreprocessor>();

            var packedGrid = pluginFrontPreprocessor.PackPluginHtmlToGrid(processedPlugins, pluginConfiguration);

            Assert.That(packedGrid,Is.Not.Empty);
        }

    }
}
