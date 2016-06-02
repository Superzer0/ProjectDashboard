using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Dashboard.Controllers.API;
using Dashboard.Infrastructure.Identity.Managers;
using Dashboard.Models.Account;
using Dashboard.UI.Objects.Auth;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Moq;
using NUnit.Framework;
using UnitTests.Utils;

namespace UnitTests.Dashboard.UI.Users.AccountControllerTests
{
    [TestFixture]
    public class AccountControllerActionChangeRolesTests : BaseTestFixture
    {
        [Test]
        public async Task ModelInvalid_BadRequestReturned()
        {
            //arrange
            var accountController = AutoMock.Create<AccountController>();
            accountController.ModelState.AddModelError("some", "error");

            //act
            var actionResult = await accountController.ChangeRoles("userId", new ChangeRolesViewModel());

            //assert
            Assert.That(actionResult, Is.TypeOf<InvalidModelStateResult>());
        }

        [Test]
        public async Task UserNotFound_404Returned()
        {
            // arrange
            var userManagerMock = new Mock<ApplicationUserManager>();
            userManagerMock.Setup(p => p.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult((DashboardUser)null));

            var accountController = AutoMock.Create<AccountController>();
            accountController.UserManager = userManagerMock.Object;

            var actionResult = await accountController.ChangeRoles("userId", new ChangeRolesViewModel());

            Assert.That(actionResult, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task RolesInvalid_BadRequestReturned()
        {
            //arrange
            var userManagerMock = new Mock<ApplicationUserManager>();
            userManagerMock.Setup(p => p.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new DashboardUser()));

            var roleManagerMock = new Mock<ApplicationRoleManager>();
            roleManagerMock.Setup(p => p.Roles).Returns(new List<IdentityRole>
            {
                new IdentityRole("some role"),
                new IdentityRole("some new role"),
                new IdentityRole("another role"),
            }.AsQueryable());

            var inputRolesViewModel = new ChangeRolesViewModel
            {
                RolesToAdd = new[] { "", "non existent" },
                RolesToRemove = new[] { "", "another role", "some new role", null },
            };

            var accountController = AutoMock.Create<AccountController>();
            accountController.UserManager = userManagerMock.Object;
            accountController.RoleManager = roleManagerMock.Object;

            var actionResult = await accountController.ChangeRoles("non existent user id", inputRolesViewModel);

            Assert.That(actionResult, Is.TypeOf<InvalidModelStateResult>());
        }

        [Test]
        public async Task ErrorWhenAddingRemovingRoles_BadRequestReturned()
        {
            //arrange
            var userManagerMock = new Mock<ApplicationUserManager>();
            userManagerMock.Setup(p => p.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new DashboardUser()));

            userManagerMock.Setup(p => p.RemoveFromRoleAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new IdentityResult("some", "errors")));

            userManagerMock.Setup(p => p.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new IdentityResult("another", "error")));

            var roleManagerMock = new Mock<ApplicationRoleManager>();
            roleManagerMock.Setup(p => p.Roles).Returns(new List<IdentityRole>
            {
                new IdentityRole("some role"),
                new IdentityRole("some new role"),
                new IdentityRole("another role"),
            }.AsQueryable());


            var inputRolesViewModel = new ChangeRolesViewModel
            {
                RolesToAdd = new[] { "another role", "some role" },
                RolesToRemove = new[] { "some new role" }
            };

            var accountController = AutoMock.Create<AccountController>();
            accountController.UserManager = userManagerMock.Object;
            accountController.RoleManager = roleManagerMock.Object;

            var actionResult = await accountController.ChangeRoles("id", inputRolesViewModel);

            Assert.That(actionResult, Is.TypeOf<InvalidModelStateResult>());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task HappyPath(bool isInRoleOutput)
        {
            //arrange
            var userManagerMock = new Mock<ApplicationUserManager>();
            userManagerMock.Setup(p => p.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new DashboardUser()));

            var resultRolesAdded = new List<string>();
            var resultRolesRemoved = new List<string>();

            userManagerMock.Setup(p => p.RemoveFromRoleAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success)).Callback((string a, string b) =>
                {
                    resultRolesRemoved.Add(b);
                });

            userManagerMock.Setup(p => p.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success)).Callback((string a, string b) =>
                {
                    resultRolesAdded.Add(b);
                });

            userManagerMock.Setup(p => p.IsInRoleAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(isInRoleOutput));

            var roleManagerMock = new Mock<ApplicationRoleManager>();
            roleManagerMock.Setup(p => p.Roles).Returns(new List<IdentityRole>
            {
                new IdentityRole("some role"),
                new IdentityRole("some new role"),
                new IdentityRole("another role"),
            }.AsQueryable());


            var inputRolesViewModel = new ChangeRolesViewModel
            {
                RolesToAdd = new[] { "another role", "some role" },
                RolesToRemove = new[] { "some new role" }
            };

            var accountController = AutoMock.Create<AccountController>();
            accountController.UserManager = userManagerMock.Object;
            accountController.RoleManager = roleManagerMock.Object;

            var actionResult = await accountController.ChangeRoles("id", inputRolesViewModel);

            Assert.That(actionResult, Is.TypeOf<OkResult>());

            if (isInRoleOutput)
            {
                Assert.That(resultRolesAdded, Is.Empty);
                Assert.That(resultRolesRemoved, Is.EquivalentTo(inputRolesViewModel.RolesToRemove));
            }
            else
            {
                Assert.That(resultRolesAdded, Is.EquivalentTo(inputRolesViewModel.RolesToAdd));
                Assert.That(resultRolesRemoved, Is.Empty);
            }

        }
    }
}
