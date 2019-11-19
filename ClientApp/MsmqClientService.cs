using Experimental.System.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace ClientApp
{
    public class MsmqClientService : IDisposable
    {
        Thread workThread;
        ManualResetEvent stopWorkEvent;
        private const string ServerQueueName = @".\Private$\MsmqTRansferFileQueue";
        private const string ClientQueueName = @".\Private$\MsmqTRansferFileQueueClient";

        private const string DefaultPath = "C:\\DefaultFolder\\";
        private const long byteMaxSizeForChunk = 3000000;
        private string _path = string.Empty;

        private FileSystemWatcher _watcher;

        private List<Message> _messages;
      

        MessageQueue serverQueue;
        MessageQueue clientQueue;

        public MsmqClientService()
        {
            CreateQueue();

            _messages = new List<Message>();
            _watcher = new FileSystemWatcher();
            serverQueue = new MessageQueue(ServerQueueName, QueueAccessMode.Send);
            clientQueue = new MessageQueue(ClientQueueName, true);

            SetMonitoringPath();
            SetupWatcher();

            stopWorkEvent = new ManualResetEvent(false);

            //workThread = new Thread(Client);
            //workThread.Start();
        }

        private void CreateQueue()
        {
            if (!MessageQueue.Exists(ClientQueueName))
                MessageQueue.Create(ClientQueueName);
        }

        private void SetMonitoringPath()
        {
            Console.WriteLine("Do you wanna set specific folder path for monitoring files ? Y/N");
            var answer = Console.ReadLine();

            if (answer.ToLower() == "y")
            {
                Console.WriteLine("Please, write here a new path:");
                _path = Console.ReadLine();
                _watcher.Path = _path;
                Console.WriteLine("A new path was changed");
            }
            else
            {
                Console.WriteLine($"You using a default path:{DefaultPath}");
                if (!Directory.Exists(DefaultPath))
                    Directory.CreateDirectory(DefaultPath);

                _watcher.Path = DefaultPath;
            }
        }

        private void SetupWatcher()
        {
            _watcher.NotifyFilter = NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName;

            // only pdf files
            _watcher.Filter = "*.pdf";

            _watcher.Created += OnChanged;

            // Begin watching.
            _watcher.EnableRaisingEvents = true;
        }

        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            Thread.Sleep(3000);
            var fileStream = new FileStream(e.FullPath, FileMode.Open);

            if (fileStream.Length < byteMaxSizeForChunk)
            {

                var message = new Message
                {
                    BodyStream = fileStream,
                    Label = Path.GetFileName(e.FullPath),
                    Priority = MessagePriority.Normal,
                    Formatter = new BinaryMessageFormatter(),
                    AppSpecific = 100
                };


                serverQueue.Send(message);
                fileStream.Close();
            }
            else
            {
                var data = new MemoryStream();
             
                fileStream.CopyTo(data);
            
                data.Seek(0, SeekOrigin.Begin); 
                byte[] buf = new byte[data.Length];
                data.Read(buf, 0, buf.Length);

                var size = fileStream.Length;
                fileStream.Close();
                var chunkCount = (int)Math.Ceiling(size / (decimal)byteMaxSizeForChunk);
                var bufferArray = new byte[chunkCount][];

                AddMessage(new MemoryStream(Encoding.ASCII.GetBytes($"{Path.GetFileName(e.FullPath)}")), "Initial", 0);
                for (var i = 0; i < chunkCount; i++)
                {
                    bufferArray[i] = new byte[byteMaxSizeForChunk];
                    for (var j = 0; j < byteMaxSizeForChunk && i * chunkCount + j < size; j++)
                    {
                        bufferArray[i][j] = buf[i * chunkCount + j];
                    }

                    AddMessage(new MemoryStream(bufferArray[i]), i.ToString(), i);
                }

                AddMessage(new MemoryStream(Encoding.ASCII.GetBytes($"{chunkCount}")), "Last", 0); 
            }

            Console.WriteLine($"File: {e.FullPath} which was {e.ChangeType} was sent to Server.");
        }

        private void AddMessage(Stream stream, string label, int appSpecific)
        {
            var message = new Message
            {
                BodyStream = stream,
                Label = label,
                Priority = MessagePriority.Normal,
                Formatter = new BinaryMessageFormatter(),
                AppSpecific = appSpecific
            };

            serverQueue.Send(message);

            message.Dispose();
            _messages.Add(message);
        }

        private void Client(object obj)
        {
            var requestText = "Hello";

            clientQueue.Formatter = new BinaryMessageFormatter();
            clientQueue.MessageReadPropertyFilter.CorrelationId = true;

            while (!stopWorkEvent.WaitOne(TimeSpan.Zero))
            {
                var message = new Message(requestText, clientQueue.Formatter) { ResponseQueue = clientQueue };
                serverQueue.Send(message);

                var id = message.Id;
                string correlationId = "";

                while (id != correlationId)
                {
                    var asyncReceive = clientQueue.BeginPeek();

                    var res = WaitHandle.WaitAny(new WaitHandle[] { stopWorkEvent, asyncReceive.AsyncWaitHandle });
                    if (res == 0)
                        break;

                    message = clientQueue.EndPeek(asyncReceive);
                    clientQueue.ReceiveById(message.Id);
                    correlationId = message.CorrelationId;
                }

                if (id != correlationId)
                    break;

                var text = string.Format(" Received: {0} {1}!\n from the server", requestText, message.Body);

                Console.WriteLine(text);

                Thread.Sleep(1000);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _watcher = null;
                _messages = null;
            }

            serverQueue = null;
            clientQueue = null;

        }

        ~MsmqClientService()
        {
            Dispose(false);
        }
    }
}
