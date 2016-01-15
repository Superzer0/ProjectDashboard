using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Common.Logging;
using Dashboard.DataAccess;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Models.Home;

namespace Dashboard.Controllers.MVC
{
    [Authorize]
    [RoutePrefix("Home")]
    public class HomeController : RazorController
    {
        private readonly PluginsContext _pluginsContext;
        private readonly ILog _log = LogManager.GetLogger<HomeController>();

        public HomeController(PluginsContext pluginsContext)
        {
            _pluginsContext = pluginsContext;
        }

        [HttpGet]
        [Route("", Name = "DashboardHome")]
        public async Task<IHttpActionResult> Index()
        {
            var user = await UserManager.FindByNameAsync(User.Identity.Name);

            return View("~/Views/Home.cshtml",
                new HomeViewModel { User = user, Plugins = new List<PluginViewModel>() });
        }
    }
}
