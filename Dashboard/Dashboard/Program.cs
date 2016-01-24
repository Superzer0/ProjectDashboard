using System;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using Common.Logging;
using Dashboard.Infrastructure.Services.Abstract;
using Dashboard.UI.Objects.DataObjects.Install.AutoMapping;
using Dashboard.UI.Objects.Services;
using Microsoft.Owin.Hosting;

namespace Dashboard
{
    public class Program
    {
        private const string ShutDownCommand = "-shutdown";
        static void Main(string[] args)
        {
            AutoMapperConfiguration.Configure();

            if (AppDomain.CurrentDomain.IsDefaultAppDomain())
            {
                // RazorEngine cannot clean up from the default appdomain...
                SwitchAppDomain();
                return;
            }

            IEnvironment env = new OwinSelfHostEnvironment();
            LogManager.GetLogger<Program>().Info(m => m("Application version" + env.AppVersion));
            LogManager.GetLogger<Program>().Info(m => m("Application root path: " + env.AppRootPath));
            using (WebApp.Start<Application>(env.BaseAddress))
            {
                LogManager.GetLogger<Program>().Info(m => m("Dashboard server stared on " + env.BaseAddress));
                Console.WriteLine("Available commands: ");
                Console.WriteLine($"1. {ShutDownCommand} stop Dashboard server");

                while (!ShutDownCommand.Equals(Console.ReadLine(), StringComparison.OrdinalIgnoreCase)) { }

                LogManager.GetLogger<Program>().Info(m => m("Shutting down Dashboard server"));
            }
        }

        private static void SwitchAppDomain()
        {
            if (!AppDomain.CurrentDomain.IsDefaultAppDomain()) return;

            LogManager.GetLogger<Program>()
                .Info(m => m("Switching to DashboardServerAppDomain from default one for RazorEngine..."));

            var current = AppDomain.CurrentDomain;
            // You only need to add strongnames when your appdomain is not a full trust environment.
            var strongNames = new StrongName[0];

            var domain = AppDomain.CreateDomain(
                "DashboardServerAppDomain", null,
                current.SetupInformation, new PermissionSet(PermissionState.Unrestricted),
                strongNames);
            domain.ExecuteAssembly(Assembly.GetExecutingAssembly().Location);
            // RazorEngine will cleanup. 
            AppDomain.Unload(domain);
        }
    }
}