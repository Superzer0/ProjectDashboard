using AutoMapper;
using Dashboard.Common.PluginXml;
using Dashboard.UI.Objects.DataObjects.Extract;
using Dashboard.UI.Objects.DataObjects.Install.AutoMapping.Resolvers;

namespace Dashboard.UI.Objects.DataObjects.Install.AutoMapping
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.CreateMap<PluginXml, Plugin>()
                .ForMember(dest => dest.CommunicationType, opt => opt.ResolveUsing<CommunicationTypeResolver>()
                .FromMember(p => p.CommunicationType))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(p => p.Name))
                .ForMember(dest => dest.Version, opt => opt.MapFrom(p => p.Version))
                .ForMember(dest => dest.StartingProgram, opt => opt.MapFrom(p => p.StartingProgram))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(p => p.PluginId))
                .ForMember(dest => dest.Icon, opt => opt.MapFrom(p => p.Icon))

                .ForMember(dest => dest.ArchiveSize, opt => opt.Ignore())
                .ForMember(dest => dest.PluginMethods, opt => opt.Ignore())
                .ForMember(dest => dest.Added, opt => opt.Ignore())
                .ForMember(dest => dest.FilesCount, opt => opt.Ignore())
                .ForMember(dest => dest.Configuration, opt => opt.Ignore())
                .ForMember(dest => dest.Disabled, opt => opt.Ignore())
                .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Xml, opt => opt.Ignore())
                .ForMember(dest => dest.UncompressedSize, opt => opt.Ignore());


            Mapper.CreateMap<PluginXmlMethod, PluginMethod>()
                .ForMember(dest => dest.InputType, opt => opt.ResolveUsing<InputTypeResolver>().FromMember(p => p.InputType))
                .ForMember(dest => dest.OutputType, opt => opt.ResolveUsing<InputTypeResolver>().FromMember(p => p.OutputType))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(p => p.Name))
                .ForMember(dest => dest.PluginId, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PluginVersion, opt => opt.Ignore())
                .ForMember(dest => dest.Plugin, opt => opt.Ignore());

            Mapper.CreateMap<PluginZipBasicInformation, Plugin>()
                .ForMember(dest => dest.UncompressedSize, opt => opt.MapFrom(p => p.UncompressedSize))
                .ForMember(dest => dest.FilesCount, opt => opt.MapFrom(p => p.FilesCount))
                .ForMember(dest => dest.ArchiveSize, opt => opt.MapFrom(p => p.ArchiveSize))

                .ForMember(dest => dest.Icon, opt => opt.Ignore())
                .ForMember(dest => dest.CommunicationType, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Version, opt => opt.Ignore())
                .ForMember(dest => dest.StartingProgram, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PluginMethods, opt => opt.Ignore())
                .ForMember(dest => dest.Added, opt => opt.Ignore())
                .ForMember(dest => dest.Configuration, opt => opt.Ignore())
                .ForMember(dest => dest.Disabled, opt => opt.Ignore())
                .ForMember(dest => dest.Xml, opt => opt.Ignore())
                .ForMember(dest => dest.AddedBy, opt => opt.Ignore());
        }
    }
}
