using System;
using Dashboard.Infrastructure.Identity.MessageServices;
using Dashboard.UI.Objects.Auth;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;

namespace Dashboard.Infrastructure.Identity.Managers
{
    // Configure the application user manager which is used in this application.
    public class ApplicationUserManager : UserManager<DashboardUser>
    {
        internal ApplicationUserManager() 
            : base(new UserStore<DashboardUser>())
        {
            // for unit testint only
        }

        public ApplicationUserManager(IUserStore<DashboardUser> store, IDataProtectionProvider dataProtectionProvider)
            : base(store)
        {
            UserValidator = new UserValidator<DashboardUser>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true
            };

            // Configure user lockout defaults
            UserLockoutEnabledByDefault = true;
            DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<DashboardUser>
            {
                MessageFormat = "Your security code is {0}"
            });

            RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<DashboardUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });

            EmailService = new EmailService();
            SmsService = new SmsService();
            if (dataProtectionProvider != null)
            {
                UserTokenProvider = new DataProtectorTokenProvider<DashboardUser>(dataProtectionProvider.Create("ProjectDashboardUserToken"));
            }
        }
    }
}
