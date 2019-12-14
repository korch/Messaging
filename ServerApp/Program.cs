using ServerApp.Msmq;
using System;
using System.Threading;
using Autofac;
using ServerApp.Msmq.Configuration;
using ServerApp.Msmq.Configuration.Interfaces;
using ServerApp.Msmq.Configuration.Options;

namespace ServerApp
{
    class Program
    {

        private static IContainer CompositionRoot()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MsmqServer>().As<IServer>();
            builder.RegisterType<MsmqServerOptions>().As<IServerOptions>();
            builder.RegisterType<FileSystemManager>().As<IFileSystemManager>();
            builder.RegisterType<QueueCreateManager>().As<IQueueCreateManager>();
            builder.RegisterType<MessageToFilePartMapper>().As<IMessageToFilePartMapper>();
            builder.RegisterType<FileMessageBuffer>().As<IFileMessageBuffer>();
            builder.RegisterType<FileCopyManager>().As<IFileCopyManager>();
            builder.RegisterType<MsmqServiceFactory>().As<IServerFactory>();
            builder.RegisterType<ServerOptionsProvider>().As<IServerOptionsProvider>();
            builder.RegisterType<DirectoryManager>().As<IDirectory>();
            builder.RegisterType<QueueManager>().As<IQueueManager>();
            builder.RegisterType<QueueCreateManager>().As<IQueueCreateManager>();

            return builder.Build();
        }

        static void Main(string[] args)
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;

            var server = CompositionRoot().Resolve<IServerFactory>().GetServer();
            server.Run(token);

            Console.WriteLine("Press Any Key to cancel work");
            Console.ReadKey();
        }
    }
}
