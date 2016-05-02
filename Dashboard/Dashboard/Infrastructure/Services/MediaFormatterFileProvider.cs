using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.Providers;
using Dashboard.UI.Objects.Services;

namespace Dashboard.Infrastructure.Services
{
    internal class MediaStreamFileProvider : IProvideFiles
    {
        private readonly IEnvironment _environment;

        public MediaStreamFileProvider(IEnvironment environment)
        {
            _environment = environment;
        }

        public bool ValidateRequest(HttpRequestMessage request)
        {
            return request.Content.IsMimeMultipartContent();
        }

        public async Task<UploadedFileMetadata> ReceiveFile(HttpRequestMessage request)
        {
            var root = _environment.MapPath(_environment.PluginsUploadPath);
            var provider = new MultipartFormDataStreamProvider(root);

            await request.Content.ReadAsMultipartAsync(provider);

            var zipFileToProcess = provider.FileData.FirstOrDefault(); // only one plugin at a time
            if (zipFileToProcess == null) return null; // no files

            return new UploadedFileMetadata
            {
                ReceivedFileName = zipFileToProcess.Headers?.ContentDisposition?.FileName,
                LocalFileName = zipFileToProcess.LocalFileName
            };
        }
    }
}
