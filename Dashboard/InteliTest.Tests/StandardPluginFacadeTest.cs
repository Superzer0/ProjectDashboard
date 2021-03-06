using Dashboard.UI.Objects.DataObjects.Validation;
using System.Threading.Tasks;
using Dashboard.UI.Objects.Services.Plugins.Install;
using AutoMapper;
using Dashboard.UI.Objects.Providers;
using Dashboard.UI.Objects.BrokerIntegration;
using Dashboard.UI.Objects.Services.Plugins.Extract;
using Dashboard.UI.Objects.Services.Plugins.Validation;
using System;
using Dashboard.Services.Plugins;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;

namespace Dashboard.Services.Plugins.Tests
{
    /// <summary>This class contains parameterized unit tests for StandardPluginFacade</summary>
    [TestFixture]
    [PexClass(typeof(StandardPluginFacade))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class StandardPluginFacadeTest
    {

        /// <summary>Test stub for .ctor(IBuildValidationResult, IBuildPluginInfo, IManageBrokerFacade, IProvidePlugins, IManagePluginsStorage, IMapper)</summary>
        [PexMethod]
        internal StandardPluginFacade ConstructorTest(
            IBuildValidationResult validationResultBuilder,
            IBuildPluginInfo pluginInfoBuilder,
            IManageBrokerFacade brokerFacade,
            IProvidePlugins pluginsProvider,
            IManagePluginsStorage pluginsStorage,
            IMapper mapper
        )
        {
            StandardPluginFacade target
               = new StandardPluginFacade(validationResultBuilder, pluginInfoBuilder, brokerFacade,
                                          pluginsProvider, pluginsStorage, mapper);
            return target;
            // TODO: add assertions to method StandardPluginFacadeTest.ConstructorTest(IBuildValidationResult, IBuildPluginInfo, IManageBrokerFacade, IProvidePlugins, IManagePluginsStorage, IMapper)
        }

        /// <summary>Test stub for AddToValidationQueue(String, String, Guid)</summary>
        [PexMethod]
        internal bool AddToValidationQueueTest(
            [PexAssumeUnderTest]StandardPluginFacade target,
            string fileId,
            string filePath,
            Guid userId
        )
        {
            bool result = target.AddToValidationQueue(fileId, filePath, userId);
            return result;
            // TODO: add assertions to method StandardPluginFacadeTest.AddToValidationQueueTest(StandardPluginFacade, String, String, Guid)
        }

        /// <summary>Test stub for GetPluginInstallableInformationAsync(String, Guid)</summary>
        [PexMethod]
        internal Task<PluginInformation> GetPluginInstallableInformationAsyncTest(
            [PexAssumeUnderTest]StandardPluginFacade target,
            string fileId,
            Guid userId
        )
        {
            Task<PluginInformation> result = target.GetPluginInstallableInformationAsync(fileId, userId);
            return result;
            // TODO: add assertions to method StandardPluginFacadeTest.GetPluginInstallableInformationAsyncTest(StandardPluginFacade, String, Guid)
        }

        /// <summary>Test stub for InstallPluginAsync(String, Guid)</summary>
        [PexMethod]
        internal Task InstallPluginAsyncTest(
            [PexAssumeUnderTest]StandardPluginFacade target,
            string fileId,
            Guid userId
        )
        {
            Task result = target.InstallPluginAsync(fileId, userId);
            return result;
            // TODO: add assertions to method StandardPluginFacadeTest.InstallPluginAsyncTest(StandardPluginFacade, String, Guid)
        }

        /// <summary>Test stub for ValidatePluginAsync(String, Guid)</summary>
        [PexMethod]
        internal Task<ConsolidatedPluginValidationResult> ValidatePluginAsyncTest(
            [PexAssumeUnderTest]StandardPluginFacade target,
            string fileId,
            Guid userId
        )
        {
            Task<ConsolidatedPluginValidationResult> result = target.ValidatePluginAsync(fileId, userId);
            return result;
            // TODO: add assertions to method StandardPluginFacadeTest.ValidatePluginAsyncTest(StandardPluginFacade, String, Guid)
        }
    }
}
