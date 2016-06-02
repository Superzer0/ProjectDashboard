using System.Collections.Generic;
using Dashboard.Services.Plugins.Validation;
using Dashboard.UI.Objects.Services.Plugins.Validation;
using NUnit.Framework;
using UnitTests.Utils;

namespace UnitTests.Dashboard.UI.Installation.Services
{
    [TestFixture]
    public class StandardPluginExtractorBuilderTests : BaseTestFixture
    {
        [Test]
        public void test()
        {
            var standardPluginValidationBuilder = AutoMock.Create<StandardPluginValidationBuilder>();
            standardPluginValidationBuilder.ConfigureStandard();
            standardPluginValidationBuilder.ConfigureBuilder(new List<IValidatePlugin>());
        }
    }
}
