using System;
using Autofac;
using ClientApp.Configure;
using ClientApp.Configure.Interfaces;

namespace ClientApp
{
    class Program
    {
        private static IContainer CompositionRoot()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MsmqClientService>().As<IService>();
            builder.RegisterType<Watcher>().As<IWatcher>();
            builder.RegisterType<ProcessingManager>().As<IProcessingManager>();

            return builder.Build();
        }

       static void Main(string[] args)
        {
            Console.WriteLine("Msmq client app starting work...!");

            var service = CompositionRoot().Resolve<IService>();
            service.Run();
            
            Console.ReadKey(true);
        }
    }
}
