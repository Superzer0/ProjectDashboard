// <copyright file="PexAssemblyInfo.cs">Copyright ©  2016</copyright>
using Microsoft.Pex.Framework.Coverage;
using Microsoft.Pex.Framework.Creatable;
using Microsoft.Pex.Framework.Instrumentation;
using Microsoft.Pex.Framework.Settings;
using Microsoft.Pex.Framework.Validation;

// Microsoft.Pex.Framework.Settings
[assembly: PexAssemblySettings(TestFramework = "NUnit")]

// Microsoft.Pex.Framework.Instrumentation
[assembly: PexAssemblyUnderTest("Dashboard.Services")]
[assembly: PexInstrumentAssembly("Dashboard.Common")]
[assembly: PexInstrumentAssembly("System.Xml.Linq")]
[assembly: PexInstrumentAssembly("System.Core")]
[assembly: PexInstrumentAssembly("HtmlAgilityPack")]
[assembly: PexInstrumentAssembly("Newtonsoft.Json")]
[assembly: PexInstrumentAssembly("AutoMapper")]
[assembly: PexInstrumentAssembly("Dashboard.UI.Objects")]
[assembly: PexInstrumentAssembly("Common.Logging")]
[assembly: PexInstrumentAssembly("System.IO.Compression")]

// Microsoft.Pex.Framework.Creatable
[assembly: PexCreatableFactoryForDelegates]

// Microsoft.Pex.Framework.Validation
[assembly: PexAllowedContractRequiresFailureAtTypeUnderTestSurface]
[assembly: PexAllowedXmlDocumentedException]

// Microsoft.Pex.Framework.Coverage
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Dashboard.Common")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Xml.Linq")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Core")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "HtmlAgilityPack")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Newtonsoft.Json")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "AutoMapper")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Dashboard.UI.Objects")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Common.Logging")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.IO.Compression")]

