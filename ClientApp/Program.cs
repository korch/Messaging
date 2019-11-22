using System;

namespace ClientApp
{
    class Program
    {
        private const string ServerQueueName = @".\Private$\MsmqTRansferFileQueue";
        private const string ClientQueueName = @".\Private$\MsmqTRansferFileQueueClient";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var msmq = new MsmqClientService();

            msmq.Run();

            Console.ReadKey(true);
        }
    }
}
