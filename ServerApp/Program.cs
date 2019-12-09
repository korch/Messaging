using ServerApp.Msmq;
using System;
using Autofac;

namespace ServerApp
{
    class Program
    {

        private static IContainer CompositionRoot()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MsmqService>().As<IServer>();
            return builder.Build();
        }


        static void Main(string[] args)
        {
            var server = CompositionRoot().Resolve<IServer>();
            server.Run();

            Console.ReadKey();
        }
    }
}
