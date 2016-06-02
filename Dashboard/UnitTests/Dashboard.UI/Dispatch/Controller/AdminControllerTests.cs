using System.Threading.Tasks;
using Dashboard.Controllers.MVC;
using Dashboard.Infrastructure.ActionResults;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.Services;
using Moq;
using NUnit.Framework;
using UnitTests.Utils;

namespace UnitTests.Dashboard.UI.Dispatch.Controller
{
    [TestFixture]
    public class AdminControllerTests : BaseTestFixture
    {
        [Test]
        public void Index_ReturnsCorrectView()
        {
            var adminController = AutoMock.Create<AdminController>();
            var envMock = new Mock<IEnvironment>();
            adminController.Environment = envMock.Object;
            var actionResult = adminController.Index();
            Assert.That(actionResult, Is.TypeOf<RazorResult>());
        }

        [Test]
        public void Authconstants_ThrowsException()
        {
            var adminController = AutoMock.Create<AdminController>();
            var envMock = new Mock<IEnvironment>();
            adminController.Environment = envMock.Object;
            Assert.That(()=> (adminController.AuthConstants()), Throws.ArgumentNullException);
        }

        [Test]
        public void Authconstants_ReturnsCorrectView()
        {
            AutoMock.Mock<IAuthRepository>().Setup(p => p.GenerateOfficialClientId()).Returns(new AuthClient {Id = "sdfsdf"});   
            var adminController = AutoMock.Create<AdminController>();
            var envMock = new Mock<IEnvironment>();
            adminController.Environment = envMock.Object;
            var actionResult = adminController.AuthConstants();
            Assert.That(actionResult, Is.TypeOf<RazorResult>());
        }
    }
}
