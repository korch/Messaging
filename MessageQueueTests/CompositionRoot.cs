using Autofac;
using ClientApp.Configure;
using ClientApp.Configure.Interfaces;

namespace MessageQueueTests
{
    public static class CompositionRoot
    {
        public static IContainer Get()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MsmqClientService>().As<IService>();
            builder.RegisterType<Watcher>().As<IWatcher>();
            builder.RegisterType<ProcessingManager>().As<IProcessingManager>();

            return builder.Build();
        }
    }
}
