using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Common.Logging;
using Dashboard.DataAccess;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Infrastructure.Identity;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.Services.Plugins;

namespace Dashboard.Controllers.API
{
    [RoutePrefix("api/plugins")]
    [Authorize(Roles = DashboardRoles.PluginManager)]
    public class PluginsController : BaseController
    {
        private readonly IManagePluginsFacade _pluginsFacade;
        private readonly PluginsContext _pluginsContext;
        private readonly ILog _logger = LogManager.GetLogger<PluginsController>();

        public PluginsController(IManagePluginsFacade pluginsFacade, PluginsContext pluginsContext)
        {
            _pluginsFacade = pluginsFacade;
            _pluginsContext = pluginsContext;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IHttpActionResult> UploadPlugin()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var root = Environment.MapPath(Environment.PluginsUploadPath);
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                var zipFileToProcess = provider.FileData.FirstOrDefault(); // only one plugin at a time

                if (zipFileToProcess == null) return BadRequest("No file found");

                var fileExtension =
                    Path.GetExtension(zipFileToProcess.Headers.ContentDisposition.FileName
                        .Trim('"')).TrimStart('.');

                if (!"zip".Equals(fileExtension, StringComparison.OrdinalIgnoreCase))
                    return BadRequest("File must be an zip archive");

                var fileId = Path.GetFileName(zipFileToProcess.LocalFileName);

                _pluginsFacade.AddToValidationQueue(fileId, zipFileToProcess.LocalFileName);

                return Ok(new { fileId });
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("validate/{fileId}")]
        public async Task<IHttpActionResult> ValidatePlugin(string fileId)
        {
            if (string.IsNullOrWhiteSpace(fileId)) return BadRequest("fileId must not be empty");

            var pluginValidationResults = await _pluginsFacade.ValidatePluginAsync(fileId);

            if (pluginValidationResults.IsValidated)
                return Ok(pluginValidationResults);

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, pluginValidationResults));
        }

        [HttpPost]
        [Route("info/{fileId}")]
        public async Task<IHttpActionResult> CheckPluginInformation(string fileId)
        {
            if (string.IsNullOrWhiteSpace(fileId)) return BadRequest("fileId must not be empty");
            var pluginInfo = await _pluginsFacade.GetPluginInstallableInformationAsync(fileId);
            return Ok(pluginInfo);
        }

        [HttpPost]
        [Route("install/{fileId}")]
        public async Task<IHttpActionResult> InstallPlugin(string fileId)
        {
            if (string.IsNullOrWhiteSpace(fileId)) return BadRequest("fileId must not be empty");

            await _pluginsFacade.InstallPluginAsync(fileId);

            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("getsome")]
        public IHttpActionResult GetSome()
        {
            var newPluginContext = _pluginsContext;
            newPluginContext.Configuration.LazyLoadingEnabled = false;
            //var plugin = newPluginContext.Plugins.Find(newPluginId, pluginVersion);
            var methods = newPluginContext.Plugins.SelectMany(p => p.PluginMethods).Select(p => p.Name).ToList();

            var firstPlugin = newPluginContext.PluginMethods.Select(p => p.Plugin).ToList().Last();
            firstPlugin.Configuration = "ueueueuue";
            newPluginContext.SaveChanges();
            return Ok("ok");
        }
    }
}
