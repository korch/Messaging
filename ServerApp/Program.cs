using ServerApp.Msmq;
using System;

namespace ServerApp
{
    class Program
    {
     
        static void Main(string[] args)
        {

            var msmqService = new MsmqService();

            msmqService.Run();

            Console.ReadKey();
        }
    }
}
