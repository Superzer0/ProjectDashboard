﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Dashboard.Controllers.API;
using Dashboard.Infrastructure.Identity;
using Dashboard.Infrastructure.Identity.Managers;
using Dashboard.Models.Account;
using Dashboard.UI.Objects.Auth;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Moq;
using NUnit.Framework;
using UnitTests.Utils;

namespace UnitTests.Dashboard.UI.Users.AccountControllerTests
{
    [TestFixture]
    public class AccountControllerActionTests : BaseTestFixture
    {
        [Test]
        public void Logoff_HappyPath()
        {
            var authenticationManagerMock = new Mock<IAuthenticationManager>();
            authenticationManagerMock.Setup(p => p.SignOut(It.IsAny<string>()));

            var accountController = AutoMock.Create<AccountController>();
            accountController.AuthenticationManager = authenticationManagerMock.Object;

            //act
            var actionResult = accountController.LogOff();

            //assert
            Assert.That(actionResult, Is.TypeOf<OkResult>());
            authenticationManagerMock.Verify(p => p.SignOut(It.Is<string>(r => r == DefaultAuthenticationTypes.ApplicationCookie)));
        }

        [Test]
        public async Task ChangePassword_ModelStateInvalid_BadRequestReturnedWithModelErrors()
        {
            //arrange
            var accountController = AutoMock.Create<AccountController>();
            accountController.ModelState.AddModelError("some", "error");
            //act
            var register = await accountController.ChangePassword(new ChangePasswordViewModel());

            //assert
            Assert.That(register, Is.TypeOf<InvalidModelStateResult>());
        }

        [Test]
        public async Task ChangePassword_ProviderErrors_BadRequestReturned()
        {
            var expectedUser = new DashboardUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "user2"
            };

            var expectedPasswordModel = new ChangePasswordViewModel
            {
                CurrentPassword = "some pass",
                NewPassword = "new pass"
            };

            var applicationUserManager = new Mock<ApplicationUserManager>();
            applicationUserManager.Setup(p => p.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(expectedUser));

            applicationUserManager.Setup(p => p.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new IdentityResult()));

            var accountController = AutoMock.Create<AccountController>();
            accountController.UserManager = applicationUserManager.Object;

            //act
            var register = await accountController.ChangePassword(expectedPasswordModel);

            //assert
            Assert.That(register, Is.TypeOf<BadRequestResult>());

            applicationUserManager.Verify(
                p => p.ChangePasswordAsync(
                    It.Is<string>(userId => expectedUser.Id.Equals(userId)),
                    It.Is<string>(currentPassword => expectedPasswordModel.CurrentPassword.Equals(currentPassword)),
                    It.Is<string>(newPassword => expectedPasswordModel.NewPassword.Equals(newPassword))), Times.Once);
        }

        [Test]
        public async Task ChangePassword_HappyPath()
        {
            var expectedUser = new DashboardUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "user2"
            };

            var expectedPasswordModel = new ChangePasswordViewModel
            {
                CurrentPassword = "some pass",
                NewPassword = "new pass"
            };

            var applicationUserManager = new Mock<ApplicationUserManager>();
            applicationUserManager.Setup(p => p.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(expectedUser));

            applicationUserManager.Setup(p => p.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success));

            var accountController = AutoMock.Create<AccountController>();
            accountController.UserManager = applicationUserManager.Object;

            //act
            var register = await accountController.ChangePassword(expectedPasswordModel);

            //assert
            Assert.That(register, Is.TypeOf<OkResult>());

            applicationUserManager.Verify(
                p => p.ChangePasswordAsync(
                    It.Is<string>(userId => expectedUser.Id.Equals(userId)),
                    It.Is<string>(currentPassword => expectedPasswordModel.CurrentPassword.Equals(currentPassword)),
                    It.Is<string>(newPassword => expectedPasswordModel.NewPassword.Equals(newPassword))), Times.Once);
        }

        [Test]
        public void GetUserInfo_identityNull_BadRequestReturned()
        {
            var principalMock = new Mock<IPrincipal>();
            principalMock.Setup(p => p.Identity).Returns((ClaimsIdentity)null);

            var accountController = AutoMock.Create<AccountController>();
            accountController.RequestContext.Principal = principalMock.Object;

            //act
            var actionResult = accountController.GetUserInfo();
            Assert.That(actionResult, Is.TypeOf<BadRequestResult>());
        }

        [Test]
        public void GetUserInfo_HappyPath()
        {
            var expectedViewModel = new UserInfoViewModel
            {
                UserName = "gruby",
                Email = "gruby@hej.com",
                Id = Guid.NewGuid().ToString(),
                Roles = new List<string> { DashboardRoles.User, DashboardRoles.Admin }
            };

            var principalMock = new Mock<IPrincipal>();
            principalMock.Setup(p => p.Identity).Returns(
                new ClaimsIdentity(
                    new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, expectedViewModel.Id),
                        new Claim(ClaimTypes.Name, expectedViewModel.UserName),
                        new Claim(ClaimTypes.Email, expectedViewModel.Email),
                        new Claim(ClaimTypes.Role, expectedViewModel.Roles.First()),
                        new Claim(ClaimTypes.Role, expectedViewModel.Roles.Last())
                    }));

            var accountController = AutoMock.Create<AccountController>();
            accountController.RequestContext.Principal = principalMock.Object;

            //act
            var viewModel = (OkNegotiatedContentResult<UserInfoViewModel>)accountController.GetUserInfo();
            Assert.That(viewModel.Content.Email, Is.EqualTo(expectedViewModel.Email));
            Assert.That(viewModel.Content.UserName, Is.EqualTo(expectedViewModel.UserName));
            Assert.That(viewModel.Content.Id, Is.EqualTo(expectedViewModel.Id));
            Assert.That(viewModel.Content.Roles, Is.EquivalentTo(expectedViewModel.Roles));
        }

        [Test, TestCaseSource(nameof(GetUsersHappyPathTestCases))]
        public void GetUsers_HappyPath(IQueryable<DashboardUser> testUsers, IQueryable<IdentityRole> testRoles,
            List<UserInfoViewModel> expectedResult)
        {
            var applicationUserManager = new Mock<ApplicationUserManager>();
            applicationUserManager.Setup(p => p.Users)
                .Returns(testUsers);

            var roleManagerMock = new Mock<ApplicationRoleManager>(new Mock<IRoleStore<IdentityRole, string>>().Object);
            roleManagerMock.Setup(p => p.Roles)
                .Returns(testRoles);

            var accountController = AutoMock.Create<AccountController>();
            accountController.UserManager = applicationUserManager.Object;
            accountController.RoleManager = roleManagerMock.Object;

            //act
            var actionResult = accountController.GetAllUsers();

            //assert
            Assert.That(actionResult, Is.TypeOf<OkNegotiatedContentResult<List<UserInfoViewModel>>>());
            var resultUserList = ((OkNegotiatedContentResult<List<UserInfoViewModel>>)actionResult).Content;

            Assert.That(expectedResult.Count, Is.EqualTo(resultUserList.Count));

            for (var i = 0; i < expectedResult.Count; i++)
            {
                Assert.That(expectedResult[i].Email, Is.EqualTo(resultUserList[i].Email));
                Assert.That(expectedResult[i].Id, Is.EqualTo(resultUserList[i].Id));
                Assert.That(expectedResult[i].UserName, Is.EqualTo(resultUserList[i].UserName));
                Assert.That(expectedResult[i].Roles, Is.EquivalentTo(resultUserList[i].Roles));
            }
        }

        private static readonly object[] GetUsersHappyPathTestCases =
        {
            new object[]
            {
                new List<DashboardUser>
                {
                    new DashboardUser
                    {
                        UserName = "Damian",
                        Email = "Damian@damin.pl",
                        Id = "1",
                        Roles =
                        {
                            new IdentityUserRole {RoleId = "admin", UserId = "1"},
                            new IdentityUserRole {RoleId = "user", UserId = "1"}
                        }
                    },
                    new DashboardUser
                    {
                        UserName = "Bogdan",
                        Email = "Bogdan@Bogdan.pl",
                        Id = "2",
                        Roles =
                        {
                            new IdentityUserRole {RoleId = "user", UserId = "2"},
                            new IdentityUserRole {RoleId = "Nonexistingrole", UserId = "2"}
                        }
                    }
                }.AsQueryable(),

                new List<IdentityRole>
                {
                    new IdentityRole
                    {
                        Id = "admin",
                        Name = "ADMIN"
                    },
                    new IdentityRole
                    {
                        Id = "user",
                        Name = "USER"
                    }
                }.AsQueryable(),
                new List<UserInfoViewModel>
                {
                    new UserInfoViewModel
                    {
                        Id = "2",
                        UserName = "Bogdan",
                        Email = "Bogdan@Bogdan.pl",
                        Roles = new List<string> {"USER"}
                    },

                    new UserInfoViewModel
                    {
                        Id= "1",
                        UserName = "Damian",
                        Email = "Damian@damin.pl",
                        Roles = new List<string> {"ADMIN", "USER"}
                    }
                }
            }
        };
    }
}