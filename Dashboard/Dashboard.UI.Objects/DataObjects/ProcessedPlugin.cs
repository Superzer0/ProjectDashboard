using System;
using System.IO;

namespace Dashboard.UI.Objects.DataObjects
{
    public class ProcessedPlugin : IDisposable
    {
        private bool _isDisposed = false;
        public string FileId { get; set; }
        public Stream PluginZipStream { get; set; }

        public void ResetState()
        {
            PluginZipStream.Position = 0;
        }

        protected void Disposing(bool disposing)
        {
            if (disposing)
            {
                PluginZipStream?.Dispose();
            }
        }

        public void Dispose()
        {
            Disposing(true);
            GC.SuppressFinalize(this);
        }
    }
}
