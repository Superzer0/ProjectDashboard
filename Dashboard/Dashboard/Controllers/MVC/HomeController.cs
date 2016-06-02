using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Common.Logging;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Models.Home;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.DataObjects.Display;
using Dashboard.UI.Objects.Services;

namespace Dashboard.Controllers.MVC
{

    [RoutePrefix("home")]
    public class HomeController : RazorController
    {
        private readonly IEnvironment _environment;
        private readonly IPreparePluginFrontEnd _preparePluginFrontEnd;
        private readonly ILog _log = LogManager.GetLogger<HomeController>();

        public HomeController(IPreparePluginFrontEnd preparePluginFrontEnd, IEnvironment environment)
        {
            _preparePluginFrontEnd = preparePluginFrontEnd;
            _environment = environment;
        }

        [HttpGet]
        [Authorize(Roles = DashboardRoles.User)]
        [Route("", Name = "DashboardHome")]
        public async Task<IHttpActionResult> Index()
        {
            try
            {
                var user = await GetCurrentUser();
                var processedPluginHtmls =
                    await _preparePluginFrontEnd.ProcessActivePluginsHtml(user.Id, new HtmlProcessingOptions
                    {
                        BaseAddress = _environment.BaseAddress,
                        ResourcePrefixTag = "data-server-link",
                        ApiAppIdTag = "data-app-id"
                    });

                var processedConfiguration = await _preparePluginFrontEnd.ProcessActivePluginsConfiguration(user.Id);

                var packedGrid = _preparePluginFrontEnd.PackPluginHtmlToGrid(processedPluginHtmls, processedConfiguration);

                return View("~/Views/Home.cshtml",
                    new HomeViewModel
                    {
                        User = user,
                        PackedPlugisGrid = packedGrid,
                        ConigurationJson = GenerateConfigurationVariables(processedConfiguration)
                    });
            }
            catch (Exception e)
            {
                _log.Error(e);
                return RedirectToRoute("IndexRoute", null);
            }
        }


        public string GenerateConfigurationVariables(IEnumerable<ProcessedPluginConfiguration> configurations)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("{ ");

            foreach (var configuration in configurations)
            {
                stringBuilder.AppendLine($@"'{configuration.PluginUniqueId}' :  ");
                stringBuilder.Append(" { ");
                stringBuilder.AppendLine($" dispatchLink : '{configuration.DispatchLink}',");
                stringBuilder.AppendLine($" config : {configuration.JsonConfiguration}");
                stringBuilder.Append(" }, ");
            }

            stringBuilder.AppendLine(" }");
            return stringBuilder.ToString();
        }
    }
}
