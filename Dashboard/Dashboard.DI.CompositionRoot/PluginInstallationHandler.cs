using Autofac;
using Dashboard.Services.Plugins.Extract.Builders;
using Dashboard.Services.Plugins.Validation.Validators;
using Module = Autofac.Module;

namespace Dashboard.DI.CompositionRoot
{
    public class PluginInstallationHandler : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ZipSizeValidator>().AsSelf();
            builder.RegisterType<PluginJsonConfigurationValidator>().AsSelf();
            builder.RegisterType<PluginXmlValidator>().AsSelf();
            builder.RegisterType<PluginZipStructureValidator>().AsSelf();
            builder.RegisterType<PreviousVersionValidator>().AsSelf();

            builder.RegisterType<PluginBasicZipInformationExtractor>().AsSelf();
            builder.RegisterType<PluginJsonConfigurationExtactor>().AsSelf();
            builder.RegisterType<PluginXmlExtractor>().AsSelf();
            builder.RegisterType<CheckSumExtractor>().AsSelf();
            
        }
    }
}
