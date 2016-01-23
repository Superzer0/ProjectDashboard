using System.Collections.Generic;
using System.Linq;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Validation;
using Dashboard.UI.Objects.Services.Plugins.Validation;

namespace Dashboard.Services.Plugins.Validation.Validators
{
    internal class ZipSizeValidator : IValidatePlugin
    {
        private readonly ZipHelper _zipHelper;

        public ZipSizeValidator(ZipHelper zipHelper)
        {
            _zipHelper = zipHelper;
        }

        private const long MaxFileSize = 100 * 1000 * 1000; //100 mb

        public string Name => "ZipSizeValidator";

        public PluginValidationResult Validate(ProcessedPlugin processedPlugin)
        {
            using (var zipArchive = _zipHelper.GetZipArchiveFromStream(processedPlugin.PluginZipStream))
            {
                var zipSize = zipArchive.Entries.Aggregate(0L, (p, r) => p + r.CompressedLength);
                var validationResults = new List<string>();

                var result = new PluginValidationResult
                {
                    ValidatorName = Name,
                    IsSuccess = false,
                    ValidationResults = validationResults
                };

                if (zipSize <= 0)
                {
                    result.IsSuccess = false;
                    validationResults.Add(
                        "Plugin zip is empty");
                    return result;
                }

                if (zipSize > MaxFileSize)
                {
                    result.IsSuccess = false;
                    validationResults.Add(
                        $"Plugin zip file size ({zipSize / (1000.0 * 1000)} mb) is too big. File size allowed: 100 mb");
                    return result;
                }

                result.IsSuccess = true;

                return result;
            }
        }
    }
}
