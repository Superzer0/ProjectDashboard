using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Common.Logging;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Models.Home;
using Dashboard.UI.Objects.DataObjects.Display;
using Dashboard.UI.Objects.Services;

namespace Dashboard.Controllers.MVC
{

    [RoutePrefix("home")]
    public class HomeController : RazorController
    {
        private readonly IEnvironment _environment;
        private readonly IPreparePluginHtml _preparePluginHtml;
        private readonly ILog _log = LogManager.GetLogger<HomeController>();

        public HomeController(IPreparePluginHtml preparePluginHtml, IEnvironment environment)
        {
            _preparePluginHtml = preparePluginHtml;
            _environment = environment;
        }

        [HttpGet]
        [Route("", Name = "DashboardHome")]
        public async Task<IHttpActionResult> Index()
        {
            try
            {
                var user = await UserManager.FindByNameAsync(User.Identity.Name);
                var processActivePluginsHtml =
                    await _preparePluginHtml.ProcessActivePluginsHtml(user.Id, new HtmlProcessingOptions
                    {
                        BaseAddress = _environment.BaseAddress,
                        ResourcePrefixTag = "dblink",
                        BaseAddressTag = "api-link"
                    });

                return View("~/Views/Home.cshtml",
                    new HomeViewModel { User = user, Plugins = processActivePluginsHtml.ToArray() });
            }
            catch (Exception e)
            {
                _log.Error(e);
                return RedirectToRoute("IndexRoute", null);
            }
        }
    }
}
