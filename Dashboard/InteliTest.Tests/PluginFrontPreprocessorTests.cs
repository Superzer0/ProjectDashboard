// <copyright file="PluginFrontPreprocessorTests.cs">Copyright ©  2016</copyright>
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dashboard.Services.Display;
using Dashboard.UI.Objects.DataObjects.Display;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;

namespace InteliTest.Tests
{
    /// <summary>This class contains parameterized unit tests for PluginFrontPreprocessor</summary>
    [PexClass(typeof(PluginFrontPreprocessor))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class PluginFrontPreprocessorTests
    {
        /// <summary>Test stub for ProcessActivePluginsConfiguration(String)</summary>
        [PexMethod]
        internal Task<IEnumerable<ProcessedPluginConfiguration>> ProcessActivePluginsConfigurationTest(
            [PexAssumeUnderTest]PluginFrontPreprocessor target,
            string userId
        )
        {
            Task<IEnumerable<ProcessedPluginConfiguration>> result
               = target.ProcessActivePluginsConfiguration(userId);
            return result;
            // TODO: add assertions to method PluginFrontPreprocessorTests.ProcessActivePluginsConfigurationTest(PluginFrontPreprocessor, String)
        }
    }
}
