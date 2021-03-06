using System.Collections.Generic;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Extract;
using AutoMapper;
using System;
using Dashboard.Services.Plugins.Install.Visitors;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;

namespace Dashboard.Services.Plugins.Install.Visitors.Tests
{
    /// <summary>This class contains parameterized unit tests for CombinePluginInformationVisitor</summary>
    [TestFixture]
    [PexClass(typeof(CombinePluginInformationVisitor))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class CombinePluginInformationVisitorTest
    {

        /// <summary>Test stub for .ctor(IMapper)</summary>
        [PexMethod]
        internal CombinePluginInformationVisitor ConstructorTest(IMapper mapper)
        {
            CombinePluginInformationVisitor target = new CombinePluginInformationVisitor(mapper);
            return target;
            // TODO: add assertions to method CombinePluginInformationVisitorTest.ConstructorTest(IMapper)
        }

        /// <summary>Test stub for Visit(PluginZipBasicInformation)</summary>
        [PexMethod]
        internal void VisitTest([PexAssumeUnderTest]CombinePluginInformationVisitor target, PluginZipBasicInformation leaf)
        {
            target.Visit(leaf);
            // TODO: add assertions to method CombinePluginInformationVisitorTest.VisitTest(CombinePluginInformationVisitor, PluginZipBasicInformation)
        }

        /// <summary>Test stub for Visit(PluginXmlInfo)</summary>
        [PexMethod]
        internal void VisitTest01([PexAssumeUnderTest]CombinePluginInformationVisitor target, PluginXmlInfo leaf)
        {
            target.Visit(leaf);
            // TODO: add assertions to method CombinePluginInformationVisitorTest.VisitTest01(CombinePluginInformationVisitor, PluginXmlInfo)
        }

        /// <summary>Test stub for Visit(PluginConfigurationInfo)</summary>
        [PexMethod]
        internal void VisitTest02([PexAssumeUnderTest]CombinePluginInformationVisitor target, PluginConfigurationInfo leaf)
        {
            target.Visit(leaf);
            // TODO: add assertions to method CombinePluginInformationVisitorTest.VisitTest02(CombinePluginInformationVisitor, PluginConfigurationInfo)
        }

        /// <summary>Test stub for Visit(CheckSumPluginInformation)</summary>
        [PexMethod]
        internal void VisitTest03([PexAssumeUnderTest]CombinePluginInformationVisitor target, CheckSumPluginInformation leaf)
        {
            target.Visit(leaf);
            // TODO: add assertions to method CombinePluginInformationVisitorTest.VisitTest03(CombinePluginInformationVisitor, CheckSumPluginInformation)
        }

        /// <summary>Test stub for get_Plugin()</summary>
        [PexMethod]
        internal Plugin PluginGetTest([PexAssumeUnderTest]CombinePluginInformationVisitor target)
        {
            Plugin result = target.Plugin;
            return result;
            // TODO: add assertions to method CombinePluginInformationVisitorTest.PluginGetTest(CombinePluginInformationVisitor)
        }

        /// <summary>Test stub for get_PluginMethods()</summary>
        [PexMethod]
        internal IList<PluginMethod> PluginMethodsGetTest([PexAssumeUnderTest]CombinePluginInformationVisitor target)
        {
            IList<PluginMethod> result = target.PluginMethods;
            return result;
            // TODO: add assertions to method CombinePluginInformationVisitorTest.PluginMethodsGetTest(CombinePluginInformationVisitor)
        }
    }
}
