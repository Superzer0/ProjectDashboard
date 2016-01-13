using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Common.Logging;

namespace Dashboard.Controllers.API
{
    [RoutePrefix("api/configure")]
    public class ConfigureController : ApiController
    {
        private readonly ILog _log = LogManager.GetLogger<ConfigureController>();

        [HttpPost]
        [Route("upload-plugin")]
        public async Task<IHttpActionResult> UploadPlugin()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var path = "../plugins/";
            var provider = new MultipartMemoryStreamProvider();

            try
            {
                var httpRequestMessage = await Request.Content.ReadAsMultipartAsync(provider);
                
                foreach (var file in provider.Contents)
                {
                    //_log.Info(file.Headers.ContentDispositions.FileName);
                    //_log.Info("Server file path: " + file.LocalFileName);
                }

            }
            catch (Exception)
            {
                
                throw;
            }

            return Ok();
        }
    }
}
    