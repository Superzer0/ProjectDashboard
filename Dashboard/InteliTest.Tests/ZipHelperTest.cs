using System.IO;
using System.Collections.Generic;
using System.IO.Compression;
using System;
using Dashboard.Services.Plugins;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;

namespace Dashboard.Services.Plugins.Tests
{
    /// <summary>This class contains parameterized unit tests for ZipHelper</summary>
    [TestFixture]
    [PexClass(typeof(ZipHelper))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class ZipHelperTest
    {

        /// <summary>Test stub for EntryNonEmpty(ZipArchive, String, ICollection`1&lt;String&gt;)</summary>
        [PexMethod]
        internal bool EntryNonEmptyTest(
            [PexAssumeUnderTest]ZipHelper target,
            ZipArchive zipArchive,
            string entryName,
            ICollection<string> validationResults
        )
        {
            bool result = target.EntryNonEmpty(zipArchive, entryName, validationResults);
            return result;
            // TODO: add assertions to method ZipHelperTest.EntryNonEmptyTest(ZipHelper, ZipArchive, String, ICollection`1<String>)
        }

        /// <summary>Test stub for GetEntry(ZipArchive, String)</summary>
        [PexMethod]
        internal ZipArchiveEntry GetEntryTest(
            [PexAssumeUnderTest]ZipHelper target,
            ZipArchive zipArchive,
            string path
        )
        {
            ZipArchiveEntry result = target.GetEntry(zipArchive, path);
            return result;
            // TODO: add assertions to method ZipHelperTest.GetEntryTest(ZipHelper, ZipArchive, String)
        }

        /// <summary>Test stub for GetPluginXsdSchema()</summary>
        [PexMethod]
        internal string GetPluginXsdSchemaTest([PexAssumeUnderTest]ZipHelper target)
        {
            string result = target.GetPluginXsdSchema();
            return result;
            // TODO: add assertions to method ZipHelperTest.GetPluginXsdSchemaTest(ZipHelper)
        }

        /// <summary>Test stub for GetZipArchiveFromStream(Stream)</summary>
        [PexMethod]
        internal ZipArchive GetZipArchiveFromStreamTest([PexAssumeUnderTest]ZipHelper target, Stream stream)
        {
            ZipArchive result = target.GetZipArchiveFromStream(stream);
            return result;
            // TODO: add assertions to method ZipHelperTest.GetZipArchiveFromStreamTest(ZipHelper, Stream)
        }
    }
}
