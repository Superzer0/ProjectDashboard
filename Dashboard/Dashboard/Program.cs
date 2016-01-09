using System;
using Common.Logging;
using Dashboard.Infrastructure;
using Microsoft.Owin.Hosting;

namespace Dashboard
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (WebApp.Start<Application>(AppConfiguration.LocalAddress))
            {   
                LogManager.GetLogger<Program>().Info(m => m("Dashboard server stared"));
                Console.ReadLine();
            }
        }
    }
}