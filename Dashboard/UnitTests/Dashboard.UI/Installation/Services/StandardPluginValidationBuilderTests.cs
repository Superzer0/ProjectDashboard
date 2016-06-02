using System.Collections.Generic;
using Dashboard.Services.Plugins.Extract;
using Dashboard.Services.Plugins.Validation;
using Dashboard.UI.Objects.DataObjects.Extract;
using Dashboard.UI.Objects.Services.Plugins.Extract;
using Dashboard.UI.Objects.Services.Plugins.Validation;
using NUnit.Framework;
using UnitTests.Utils;

namespace UnitTests.Dashboard.UI.Installation.Services
{
    [TestFixture]
    public class StandardPluginValidationBuilderTests : BaseTestFixture
    {
        [Test]
        public void test()
        {
            var standardPluginValidationBuilder = AutoMock.Create<StandardPluginInfoBuilder>();
            standardPluginValidationBuilder.ConfigureStandard();
            standardPluginValidationBuilder.ConfigureBuilder(new List<IExtractPluginInformation<BasePluginInformation>>());
        }
    }
}
