using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Dashboard.Common.PluginSchema;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Validation;
using Dashboard.UI.Objects.Services.Plugins.Validation;

namespace Dashboard.Services.Plugins.Validation.Validators
{
    internal class PluginXmlValidator : IValidatePlugin
    {
        private readonly ZipHelper _zipHelper;

        public PluginXmlValidator(ZipHelper zipHelper)
        {
            _zipHelper = zipHelper;
        }

        public string Name => "PluginXmlValidator";

        public PluginValidationResult Validate(ProcessedPlugin processedPlugin)
        {
            using (var zipArchive = _zipHelper.GetZipArchiveFromStream(processedPlugin.PluginZipStream))
            {
                var validationResults = new List<string>();

                var pluginXmlZipEntryValid = _zipHelper.EntryNonEmpty(zipArchive, PluginZipStructure.PluginXml, validationResults);
                var xmlValid = false;
                if (pluginXmlZipEntryValid)
                {
                    var zipEntry = zipArchive.Entries.First(p => p.FullName.Equals(PluginZipStructure.PluginXml));
                    using (var stringReader = new StreamReader(zipEntry.Open()))
                    {
                        var xsdSchema = _zipHelper.GetPluginXsdSchema();
                        var schema = new XmlSchemaSet();
                        schema.Add("", XmlReader.Create(new StringReader(xsdSchema)));

                        var document = XDocument.Load(new XmlTextReader(stringReader));

                        xmlValid = true;
                        document.Validate(schema, (o, e) =>
                        {
                            xmlValid = false;
                            validationResults.Add(e.Message);
                        });
                    }
                }

                return new PluginValidationResult
                {
                    IsSuccess = pluginXmlZipEntryValid && xmlValid,
                    ValidationResults = validationResults,
                    ValidatorName = Name
                };
            }
        }
    }
}
