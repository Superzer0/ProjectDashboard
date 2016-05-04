using System;
using AutoMapper;
using Dashboard.UI.BrokerIntegration.BrokerExecution;
using Dashboard.UI.BrokerIntegration.BrokerInstance;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Execution;

namespace Dashboard.UI.BrokerIntegration
{
    public class BrokerObjectsMappingProfile : Profile
    {
        protected override void Configure()
        {

            CreateMap<BrokerInformation, BrokerStats>()
                .ForMember(dest => dest.EndpointAddress, opt => opt.Ignore());

            CreateMap<BrokerExecutionInfo, PluginExecutionInfo>()
                .ForMember(dest => dest.ExtensionData, opt => opt.Ignore());
        }
    }
}
