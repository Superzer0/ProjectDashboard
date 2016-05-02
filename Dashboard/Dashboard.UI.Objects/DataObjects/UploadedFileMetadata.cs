using System;
using System.IO;

namespace Dashboard.UI.Objects.DataObjects
{
    public class UploadedFileMetadata
    {
        public string ReceivedFileName { get; set; }
        public string LocalFileName { get; set; }
        public bool IsZip()
        {
            if (string.IsNullOrEmpty(ReceivedFileName))
                return false;

            var fileExtension = Path.GetExtension(ReceivedFileName.Trim('"')).TrimStart('.');
            return "zip".Equals(fileExtension, StringComparison.OrdinalIgnoreCase);
        }
    }
}
