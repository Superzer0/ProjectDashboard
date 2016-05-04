using AutoMapper;
using Dashboard.UI.BrokerIntegration;
using Dashboard.UI.Objects.DataObjects.Install.AutoMapping;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class AutoMapperConfigurationTests
    {
        [Test]
        public void AssertConfiguration()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<UiObjectsMappingProfile>();
                cfg.AddProfile<BrokerObjectsMappingProfile>();
            });

            config.AssertConfigurationIsValid();
        }

    }
}
