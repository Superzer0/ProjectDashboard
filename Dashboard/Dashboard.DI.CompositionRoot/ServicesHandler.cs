using Autofac;


namespace Dashboard.DI.CompositionRoot
{
    /// <summary>
    /// sdfsdfsdfsdfasfsdf
    /// </summary>
    public class ServicesHandler : Module
    {
        public delegate void SomeFunc(object sender);

        public event SomeFunc SomeEvent;

        protected override void Load(ContainerBuilder builder)
        {
            SomeEvent(new object());
            // register dep here
        }
    }
}
