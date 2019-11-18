﻿using System;
using System.Collections.Generic;
using System.IO;
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

        public MsmqService()
        { }

        public void Run()
        {
            CreateQueue();

            stopWorkEvent = new ManualResetEvent(false);

            workThread = new Thread(Server);
            workThread.Start();
        }

        private void CreateQueue()
        {
            if (!MessageQueue.Exists(ServerQueueName))
            {
                messageQueue = MessageQueue.Create(ServerQueueName);

                Console.WriteLine($"MSMQ queue was created with name:{ServerQueueName}");
            } else
            {
                Console.WriteLine($"MSMQ queue was created with name:{ServerQueueName}");
            }
        }

        private void Server(object obj)
        {
            using (var serverQueue = new MessageQueue(ServerQueueName))
            {
                serverQueue.Formatter = new BinaryMessageFormatter();
                serverQueue.MessageReadPropertyFilter.Body = true;
                serverQueue.MessageReadPropertyFilter.CorrelationId = true;

                while (true)
                {
                    var asyncReceive = serverQueue.BeginPeek();

                    var res = WaitHandle.WaitAny(new WaitHandle[] { stopWorkEvent, asyncReceive.AsyncWaitHandle });
                    if (res == 0)
                        break;

                    var message = serverQueue.EndPeek(asyncReceive);
                    serverQueue.ReceiveById(message.Id);

                    var clientQueue = message.ResponseQueue;

                    if (clientQueue == null) {

                    } else {
                        var msg = new Message("Hello", serverQueue.Formatter) { CorrelationId = message.Id };
                        clientQueue.Send(msg);
                    }

                    byte[] buffer = new byte[1024];

                    using (FileStream output = new FileStream($"{DefaultPath}{message.Label}", FileMode.Create))
                    {
                        int readBytes = 0;
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
            }
        }
    }
}
