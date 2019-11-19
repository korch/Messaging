using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using Experimental.System.Messaging;

namespace ServerApp.Msmq
{
    public class MsmqService
    {
        Thread workThread;
        ManualResetEvent stopWorkEvent;

        private const string ServerQueueName = @".\Private$\MsmqTRansferFileQueue";
        private const string DefaultPath = "C:\\DefaultServerFolder\\";

        private MessageQueue messageQueue;

        private List<Message> _messages;

        public MsmqService()
        { }

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
                messageQueue = MessageQueue.Create(ServerQueueName);
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

                while (true)
                {
                    var asyncReceive = serverQueue.BeginPeek();

                    var res = WaitHandle.WaitAny(new WaitHandle[] { stopWorkEvent, asyncReceive.AsyncWaitHandle });
                    if (res == 0)
                        break;

                    var message = serverQueue.EndPeek(asyncReceive);
                    serverQueue.ReceiveById(message.Id);

                    var clientQueue = message.ResponseQueue;

                    if (clientQueue == null)
                    {

                    }
                    else
                    {
                        var msg = new Message("Hello", serverQueue.Formatter) { CorrelationId = message.Id };
                        clientQueue.Send(msg);
                    }

                    byte[] buffer;
                    if (message.AppSpecific == 100)
                    {
                        using (FileStream output = new FileStream($"{DefaultPath}{message.Label}", FileMode.Create))
                        {
                            int readBytes = 0;
                            buffer = new byte[message.BodyStream.Length];
                            while ((readBytes = message.BodyStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                output.Write(buffer, 0, readBytes);
                            }

                            output.Close();             
                        }

                        var text = string.Format($"Received file with name {message.Label}\n from the client");
                        Console.WriteLine(text);
                        message.Dispose();
                    }
                    else
                    {
                        if (message.AppSpecific == -2)
                        {
                            var list = _messages.Where(m => m.AppSpecific != -1);

                            using (FileStream output = new FileStream($"{DefaultPath}{message.Label}", FileMode.Create))
                            {
                                foreach (var item in list)
                                {
                                    int readBytes = 0;
                                    buffer = new byte[item.BodyStream.Length];
                                    while ((readBytes = item.BodyStream.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        output.Write(buffer, 0, readBytes);
                                    }

                                    item.Dispose();
                                }
                                output.Close();
                            }

                            var text = string.Format($"Received file with name {message.Label}\n from the client");
                            Console.WriteLine(text);
                            message.Dispose();
                            //var stream = new MemoryStream();
                            //foreach (var item in list)
                            //{    
                            //    item.BodyStream.CopyTo(stream);
                            //}

                            //buffer = new byte[stream.Length + 100];
                            //using (FileStream output = new FileStream($"{DefaultPath}{message.Label}", FileMode.Create))
                            //{
                            //    int readBytes = 0;
                            //    while ((readBytes = stream.Read(buffer, 0, buffer.Length)) > 0)
                            //    {
                            //        output.Write(buffer, 0, readBytes);
                            //    }

                            //    output.Close();

                            //    var text = string.Format($"Received file with name {message.Label}\n from the client");
                            //    Console.WriteLine(text);
                            //}

                        } else
                            _messages.Add(message);    
                    }
                }
            }
        } 
    }
}
