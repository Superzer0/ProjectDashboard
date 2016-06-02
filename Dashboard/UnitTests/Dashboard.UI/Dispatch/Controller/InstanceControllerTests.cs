using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Dashboard.Controllers.API;
using Dashboard.UI.Objects.BrokerIntegration;
using Dashboard.UI.Objects.DataObjects;
using NUnit.Framework;
using UnitTests.Utils;

namespace UnitTests.Dashboard.UI.Dispatch.Controller
{
    [TestFixture]
    public class InstanceControllerTests : BaseTestFixture
    {
        [Test]
        public void GetStatusHappyPath()
        {
            AutoMock.Mock<IManageBrokerFacade>().Setup(p => p.GetBrokerInformation()).Returns(new BrokerStats());
            var applicationAccessController = AutoMock.Create<InstanceController>();
            var httpActionResult = applicationAccessController.GetBrokerStatus();
            Assert.That(httpActionResult, Is.TypeOf<OkNegotiatedContentResult<BrokerStats>>());
        }

        [Test]
        public void GetStatus_Exception()
        {
            AutoMock.Mock<IManageBrokerFacade>().Setup(p => p.GetBrokerInformation()).Throws(new ArgumentException());
            var applicationAccessController = AutoMock.Create<InstanceController>();
            var httpActionResult = applicationAccessController.GetBrokerStatus();
            Assert.That(httpActionResult, Is.TypeOf<InternalServerErrorResult>());
        }
    }
}
