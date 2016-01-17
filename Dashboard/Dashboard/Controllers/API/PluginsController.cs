using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Common.Logging;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Infrastructure.Identity;

namespace Dashboard.Controllers.API
{
    [RoutePrefix("api/plugins")]
    [Authorize(Roles = DashboardRoles.PluginManager)]
    public class PluginsController : BaseController
    {
        private readonly ILog _logger = LogManager.GetLogger<PluginsController>();

        [Route("upload")]
        [HttpPost]
        public async Task<HttpResponseMessage> UploadPlugin()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var root = Environment.MapPath("~/plugins");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    _logger.Info(file.Headers.ContentDisposition.FileName);
                    _logger.Info("Server file path: " + file.LocalFileName);
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}
