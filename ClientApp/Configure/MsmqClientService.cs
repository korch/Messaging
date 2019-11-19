using ClientApp.Configure;
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
        private const string ServerQueueName = @".\Private$\MsmqTRansferFileQueue";
        private const long byteMaxSizeForChunk = 3000000;
    
        private Watcher _watcher;

        MessageQueue serverQueue;
   
        public MsmqClientService()
        {
            _watcher = new Watcher();
            _watcher.SetCreateHandler(OnChanged);
            serverQueue = new MessageQueue(ServerQueueName, QueueAccessMode.Send);  
        }

        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            Thread.Sleep(3000);
            var fileStream = new FileStream(e.FullPath, FileMode.Open);

            if (fileStream.Length < byteMaxSizeForChunk)
                ProcessingSingleMessage(e.FullPath, fileStream);
            else
            {
                ProcessingMultipleMessage(e.FullPath, fileStream);
            }

            Console.WriteLine($"File: {e.FullPath} which was {e.ChangeType} was sent to Server.");
        }

        /// <summary>
        /// Process single message. When we shouln't split a byte array for chunks
        /// </summary>
        /// <param name="fullPath"></param>
        private void ProcessingSingleMessage(string fullPath, FileStream fileStream)
        {
            var message = new Message {
                BodyStream = fileStream,
                Label = Path.GetFileName(fullPath),
                Priority = MessagePriority.Normal,
                Formatter = new BinaryMessageFormatter(),
                AppSpecific = 100
            };

            serverQueue.Send(message);
            fileStream.Close();
        }

        /// <summary>
        /// We have a restriction for 4MB to send a message. So, if we have files more then 4MB size --> we should split our byte array to smaller chunks and send all these chunks.
        /// </summary>
        /// <param name="fullPath"></param>
        private void ProcessingMultipleMessage(string fullPath, FileStream fileStream)
        {            
            fileStream.Seek(0, SeekOrigin.Begin);
            byte[] buf = new byte[fileStream.Length];
            fileStream.Read(buf, 0, buf.Length);

            var size = fileStream.Length;
            fileStream.Close();
            var chunkCount = (int)Math.Ceiling(size / (decimal)byteMaxSizeForChunk);
            var bufferArray = new byte[chunkCount][];

            SendMessage(new MemoryStream(Encoding.ASCII.GetBytes($"{Path.GetFileName(fullPath)}")), "Initial", -1);

            for (var i = 0; i < chunkCount; i++) {
                if (i == chunkCount - 1) {
                    bufferArray[i] = new byte[Math.Min(byteMaxSizeForChunk, size - i * byteMaxSizeForChunk)];
                    for (var j = 0; j < bufferArray[i].Length && i * chunkCount + j < size; j++) {
                        bufferArray[i][j] = buf[i * chunkCount + j];
                    }
                } else {
                    bufferArray[i] = new byte[byteMaxSizeForChunk];
                    for (var j = 0; j < byteMaxSizeForChunk && i * chunkCount + j < size; j++) {
                        bufferArray[i][j] = buf[i * chunkCount + j];
                    }
                }
                SendMessage(new MemoryStream(bufferArray[i]), i.ToString(), i);
            }
            SendMessage(new MemoryStream(Encoding.ASCII.GetBytes($"{chunkCount}")), $"{Path.GetFileName(fullPath)}", -2);
        }

        /// <summary>
        /// Send message to server
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="label"></param>
        /// <param name="appSpecific"></param>
        private void SendMessage(Stream stream, string label, int appSpecific)
        {
            var message = new Message {
                BodyStream = stream,
                Label = label,
                Priority = MessagePriority.Normal,
                Formatter = new BinaryMessageFormatter(),
                AppSpecific = appSpecific
            };

            serverQueue.Send(message);

            message.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) {
                _watcher = null;
            }

            serverQueue = null;
        }

        ~MsmqClientService()
        {
            Dispose(false);
        }
    }
}
