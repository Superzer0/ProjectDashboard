using System.Collections.Generic;
using System.Linq;
using Dashboard.Infrastructure.Identity.Managers;
using Dashboard.Infrastructure.Startup;
using Microsoft.AspNet.Identity.EntityFramework;
using Moq;
using NUnit.Framework;
using UnitTests.Utils;

namespace UnitTests.Dashboard.UI.Startup
{
    [TestFixture]
    public class AddStandardRolesTests : BaseTestFixture
    {
        [Test]
        public void Execute()
        {
            var roleManagerMock = new Mock<ApplicationRoleManager>();
            roleManagerMock.Setup(p => p.Roles).Returns(new List<IdentityRole>
            {
                new IdentityRole("some role"),
                new IdentityRole("some new role"),
                new IdentityRole("another role"),
            }.AsQueryable());

            var addStandardRoles = new AddStandardRoles(()=> roleManagerMock.Object);

            addStandardRoles.Execute();
        }
      }
}
