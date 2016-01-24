using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Common.Logging;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Infrastructure.Identity;
using Dashboard.UI.Objects.Services.Plugins;

namespace Dashboard.Controllers.API
{
    [RoutePrefix("api/plugins")]
    [Authorize(Roles = DashboardRoles.PluginManager)]
    public class PluginInstallationController : BaseController
    {
        private readonly IManagePluginsFacade _pluginsFacade;
        private readonly ILog _logger = LogManager.GetLogger<PluginInstallationController>();

        public PluginInstallationController(IManagePluginsFacade pluginsFacade)
        {
            _pluginsFacade = pluginsFacade;
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

                var user = await GetCurrentUser();
                if (user == null) return BadRequest("User not found");

                _pluginsFacade.AddToValidationQueue(fileId, zipFileToProcess.LocalFileName, Guid.Parse(user.Id));

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

            var user = await GetCurrentUser();

            var pluginValidationResults = await _pluginsFacade.ValidatePluginAsync(fileId, Guid.Parse(user.Id));

            if (pluginValidationResults.IsValidated)
                return Ok(pluginValidationResults);

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, pluginValidationResults));
        }

        [HttpPost]
        [Route("info/{fileId}")]
        public async Task<IHttpActionResult> CheckPluginInformation(string fileId)
        {
            if (string.IsNullOrWhiteSpace(fileId)) return BadRequest("fileId must not be empty");

            var user = await GetCurrentUser();
            var pluginInfo = await _pluginsFacade.GetPluginInstallableInformationAsync(fileId, Guid.Parse(user.Id));

            if (pluginInfo == null)
                return BadRequest($"transaction {fileId} not found or user does not have permission");

            return Ok(pluginInfo);
        }

        [HttpPost]
        [Route("install/{fileId}")]
        public async Task<IHttpActionResult> InstallPlugin(string fileId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileId)) return BadRequest("fileId must not be empty");

                var user = await GetCurrentUser();
                await _pluginsFacade.InstallPluginAsync(fileId, Guid.Parse(user.Id));

                return Ok();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
