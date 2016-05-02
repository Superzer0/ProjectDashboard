using System.Net.Http;
using System.Threading.Tasks;
using Dashboard.UI.Objects.DataObjects;

namespace Dashboard.UI.Objects.Providers
{
    public interface IProvideFiles
    {
        /// <summary>
        /// Validates request. Checks if is valid multipart request that can contain files
        /// </summary>
        /// <param name="request">HttpRequest Message</param>
        /// <returns>Returns true if request can contain files</returns>
        bool ValidateRequest(HttpRequestMessage request);

        /// <summary>
        /// Saves first file uploaded by user in HttpRequestMessage and returns file metadata.
        /// </summary>
        /// <param name="request">User Request Message</param>
        /// <returns>Uploaded File Metadata</returns>
        Task<UploadedFileMetadata> ReceiveFile(HttpRequestMessage request);
    }
}
