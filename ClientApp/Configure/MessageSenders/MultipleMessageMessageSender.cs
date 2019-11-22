using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Experimental.System.Messaging;

namespace ClientApp.Configure.MessageSenders
{
    public class MultipleMessageMessageSender : IMessageSender
    {
        private readonly string _queueName;
        private long byteMaxSizeForChunk = 0;

        public MultipleMessageMessageSender(string messageQueuePath, long chunkSize)
        {
            _queueName = messageQueuePath;
            byteMaxSizeForChunk = chunkSize;
        }

        /// <summary>
        /// We have a restriction for 4MB to send a message. So, if we have files more then 4MB size --> we should split our byte array to smaller chunks and send all these chunks.
        /// </summary>
        /// <param name="path"></param>
        public void SendFile(string path, Stream stream)
        {
            using (var serverQueue = new MessageQueue(_queueName, QueueAccessMode.Send)) {
                stream.Seek(0, SeekOrigin.Begin);
                byte[] buf = new byte[stream.Length];
                stream.Read(buf, 0, buf.Length);

                var size = stream.Length;
                stream.Close();
                var chunkCount = (int)Math.Ceiling(size / (decimal)byteMaxSizeForChunk);
                var bufferArray = new byte[chunkCount][];

                SendMessage(serverQueue, new MemoryStream(Encoding.ASCII.GetBytes($"{Path.GetFileName(path)}")), "Initial", -1);

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
                    SendMessage(serverQueue, new MemoryStream(bufferArray[i]), i.ToString(), i);
                }
                SendMessage(serverQueue, new MemoryStream(Encoding.ASCII.GetBytes($"{chunkCount}")), $"{Path.GetFileName(path)}", -2);
            }   
        }

        /// <summary>
        /// Send message to server
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="stream"></param>
        /// <param name="label"></param>
        /// <param name="appSpecific"></param>
        private void SendMessage(MessageQueue queue, Stream stream, string label, int appSpecific)
        {
            var message = new Message {
                BodyStream = stream,
                Label = label,
                Priority = MessagePriority.Normal,
                Formatter = new BinaryMessageFormatter(),
                AppSpecific = appSpecific
            };

            queue.Send(message);

            message.Dispose();
        }
    }
}
