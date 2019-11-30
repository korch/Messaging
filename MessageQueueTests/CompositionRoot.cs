using Autofac;
using ClientApp.Configure;
using ClientApp.Configure.Interfaces;
using ClientApp.Configure.MessageSenders;

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
            builder.RegisterType<SingleMessageSender>().As<IMessageSender>();
            builder.RegisterType<MultipleMessageSender>().As<IMessageSender>();

            return builder.Build();
        }
    }
}
