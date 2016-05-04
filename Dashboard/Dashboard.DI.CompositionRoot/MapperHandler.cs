using Autofac;
using AutoMapper;
using Dashboard.UI.BrokerIntegration;
using Dashboard.UI.Objects.DataObjects.Install.AutoMapping;

namespace Dashboard.DI.CompositionRoot
{
    public class MapperHandler : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<UiObjectsMappingProfile>();
                cfg.AddProfile<BrokerObjectsMappingProfile>();
            });

            builder.Register(p => config.CreateMapper()).SingleInstance();
        }
    }
}
