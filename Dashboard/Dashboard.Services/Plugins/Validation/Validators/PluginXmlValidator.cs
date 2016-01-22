using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Dashboard.Common.PluginSchema;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Validation;

namespace Dashboard.Services.Plugins.Validation.Validators
{
    internal class PluginXmlValidator : BasePluginValidator
    {
        public override string Name => "PluginXmlValidator";

        public override PluginValidationResult Validate(ProcessedPlugin processedPlugin)
        {
            using (var zipArchive = new ZipArchive(processedPlugin.PluginZipStream, ZipArchiveMode.Read, true))
            {
                var validationResults = new List<string>();

                var pluginXmlZipEntryValid = CheckEntryNonEmpty(zipArchive, PluginZipStructure.PluginXml, validationResults);
                var xmlValid = false;
                if (pluginXmlZipEntryValid)
                {
                    var zipEntry = zipArchive.Entries.First(p => p.FullName.Equals(PluginZipStructure.PluginXml));
                    using (var stringReader = new StreamReader(zipEntry.Open()))
                    {
                        var xsdSchema = GetPluginXsdSchema();
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
