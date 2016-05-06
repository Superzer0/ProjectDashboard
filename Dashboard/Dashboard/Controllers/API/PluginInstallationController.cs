using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Common.Logging;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Infrastructure.Identity;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.Providers;
using Dashboard.UI.Objects.Services.Plugins;
using Dashboard.UI.Resources;

namespace Dashboard.Controllers.API
{
    [RoutePrefix("api/plugins")]
    [Authorize(Roles = DashboardRoles.PluginManager)]
    public class PluginInstallationController : BaseController
    {
        private readonly IManagePluginsFacade _pluginsFacade;
        private readonly IProvideFiles _provideFiles;
        private readonly ILog _logger = LogManager.GetLogger<PluginInstallationController>();

        public PluginInstallationController(IManagePluginsFacade pluginsFacade, IProvideFiles provideFiles)
        {
            _pluginsFacade = pluginsFacade;
            _provideFiles = provideFiles;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IHttpActionResult> UploadPlugin()
        {
            // Check if the request contains multipart/form-data.
            if (!_provideFiles.ValidateRequest(Request))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            try
            {
                var user = await GetCurrentUser();
                if (user == null) return BadRequest(ExceptionMessages.UserNotFoundMessage);

                // Read the form data.
                var receivedFileMetadata = await _provideFiles.ReceiveFile(Request);
                if (receivedFileMetadata == null) return BadRequest(ExceptionMessages.FilenNotFoundMessage);

                if (!receivedFileMetadata.IsZip())
                    return BadRequest(ExceptionMessages.FileNotZipMessage);

                var fileId = Path.GetFileName(receivedFileMetadata.LocalFileName);

                _pluginsFacade.AddToValidationQueue(fileId, receivedFileMetadata.LocalFileName, Guid.Parse(user.Id));

                return Ok(new Dictionary<string, string> { { "fileId", fileId } });
            }
            catch (Exception e)
            {
                _logger.Error(m => m("Unexpected error"), e);
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("validate/{fileId}")]
        public async Task<IHttpActionResult> ValidatePlugin(string fileId)
        {
            if (string.IsNullOrWhiteSpace(fileId)) return BadRequest(ExceptionMessages.FileIdInvalid);

            var user = await GetCurrentUser();
            if (user == null) return BadRequest(ExceptionMessages.UserNotFoundMessage);

            var pluginValidationResults = await _pluginsFacade.ValidatePluginAsync(fileId, Guid.Parse(user.Id));

            if (pluginValidationResults == null)
            {
                _logger.Error(m => m("pluginValidationResults was null - internal error"), new NullReferenceException());
                return InternalServerError();
            }

            if (pluginValidationResults.IsValidated)
                return Ok(pluginValidationResults);

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, pluginValidationResults));
        }

        [HttpPost]
        [Route("info/{fileId}")]
        public async Task<IHttpActionResult> CheckPluginInformation(string fileId)
        {
            if (string.IsNullOrWhiteSpace(fileId)) return BadRequest(ExceptionMessages.FileIdInvalid);

            var user = await GetCurrentUser();
            if (user == null) return BadRequest(ExceptionMessages.UserNotFoundMessage);

            var pluginInfo = await _pluginsFacade.GetPluginInstallableInformationAsync(fileId, Guid.Parse(user.Id));

            if (pluginInfo == null)
                return BadRequest($"transaction with id: {fileId} not found");

            return Ok(pluginInfo);
        }

        [HttpPost]
        [Route("install/{fileId}")]
        public async Task<IHttpActionResult> InstallPlugin(string fileId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileId)) return BadRequest(ExceptionMessages.FileIdInvalid);

                var user = await GetCurrentUser();
                if (user == null) return BadRequest(ExceptionMessages.UserNotFoundMessage);

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
