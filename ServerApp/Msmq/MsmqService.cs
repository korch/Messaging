using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Experimental.System.Messaging;

namespace ServerApp.Msmq
{
    public class MsmqService : IServer
    {
        Thread workThread;
        ManualResetEvent stopWorkEvent;

        private const string ServerQueueName = @".\Private$\MsmqTRansferFileQueue";
        private const string DefaultPath = "C:\\DefaultServerFolder\\";

        private List<Message> _messages;

        public void Run()
        {
            CreateQueue();

            stopWorkEvent = new ManualResetEvent(false);
            _messages = new List<Message>();

            workThread = new Thread(Server);
            workThread.Start();
        }

        private void CreateQueue()
        {
            if (!MessageQueue.Exists(ServerQueueName)) {
                MessageQueue.Create(ServerQueueName);
                Console.WriteLine($"MSMQ queue was created with name:{ServerQueueName}");
            } else {
                Console.WriteLine($"You uses MSMQ with name:{ServerQueueName}");      
            }
        }

        private void Server(object obj)
        {
            using (var serverQueue = new MessageQueue(ServerQueueName))
            {
                serverQueue.Formatter = new BinaryMessageFormatter();
                serverQueue.MessageReadPropertyFilter.Body = true;
                serverQueue.MessageReadPropertyFilter.CorrelationId = true;
                serverQueue.MessageReadPropertyFilter.AppSpecific = true;

                while (true) {
                    var asyncReceive = serverQueue.BeginPeek();

                    var res = WaitHandle.WaitAny(new WaitHandle[] { stopWorkEvent, asyncReceive.AsyncWaitHandle });
                    if (res == 0)
                        break;

                    var message = serverQueue.EndPeek(asyncReceive);
                    serverQueue.ReceiveById(message.Id);

                    if (message.AppSpecific == 100) {
                        using (FileStream output = new FileStream($"{DefaultPath}{message.Label}", FileMode.Create)) {
                           Read(message.BodyStream, output);
                            output.Close();             
                        }
                        message.Dispose();

                        var text = string.Format($"Received file with name {message.Label}\n from the client");
                        Console.WriteLine(text);
                    } else {
                        if (message.AppSpecific == -2) {
                            var list = _messages.Where(m => m.AppSpecific != -1);

                            using (FileStream output = new FileStream($"{DefaultPath}{message.Label}", FileMode.Create)) {
                                foreach (var item in list)
                                {
                                   Read(item.BodyStream, output);
                                   item.Dispose();
                                }
                                output.Close();
                            }
                            message.Dispose();

                            var text = string.Format($"Received file with name {message.Label}\n from the client");
                            Console.WriteLine(text);
                        } else
                            _messages.Add(message);    
                    }
                }
            }
        }

        private void Read(Stream stream, FileStream output)
        {
            int readBytes = 0;
            byte[] buffer;
            buffer = new byte[stream.Length];
            while ((readBytes = stream.Read(buffer, 0, buffer.Length)) > 0) {
                output.Write(buffer, 0, readBytes);
            }
        }
    }
}
