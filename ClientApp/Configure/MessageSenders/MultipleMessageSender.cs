using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Experimental.System.Messaging;

namespace ClientApp.Configure.MessageSenders
{
    public class MultipleMessageSender : IMessageSender
    {
        private readonly string _queueName;
        private readonly long _byteMaxSizeForChunk;

        private const int FirstMessage = -1;
        private const int CommonMessage = 250;
        private const int LastMessage = -2;

        public MultipleMessageSender(string messageQueuePath, long chunkSize)
        {
            _queueName = messageQueuePath;
            _byteMaxSizeForChunk = chunkSize;
        }

        /// <summary>
        /// We have a restriction for 4MB to send a message. So, if we have files more then 4MB size --> we should split our byte array to smaller chunks and send all these chunks.
        /// </summary>
        /// <param name="path"></param>
        public bool SendFile(string path)
        {
            using (var serverQueue = new MessageQueue(_queueName, QueueAccessMode.Send)) {
                var fileStream = new FileStream(path, FileMode.Open);
                var fileName = Path.GetFileName(path);
                try {
                    fileStream.Seek(0, SeekOrigin.Begin);
                    byte[] buf = new byte[fileStream.Length];
                    fileStream.Read(buf, 0, buf.Length);

                    var size = fileStream.Length;
                    fileStream.Close();
                    var chunkCount = (int) Math.Ceiling(size / (decimal) _byteMaxSizeForChunk);
                    var bufferArray = new byte[chunkCount][];

                    SendMessage(serverQueue, Encoding.ASCII.GetBytes($"{Path.GetFileName(path)}"),
                        fileName, FirstMessage);

                    for (var i = 0; i < chunkCount; i++) {
                        if (i == chunkCount - 1) {
                            bufferArray[i] = new byte[Math.Min(_byteMaxSizeForChunk, size - i * _byteMaxSizeForChunk)];
                            for (var j = 0; j < bufferArray[i].Length && i * chunkCount + j < size; j++) {
                                bufferArray[i][j] = buf[i * chunkCount + j];
                            }
                        } else {
                            bufferArray[i] = new byte[_byteMaxSizeForChunk];
                            for (var j = 0; j < _byteMaxSizeForChunk && i * chunkCount + j < size; j++) {
                                bufferArray[i][j] = buf[i * chunkCount + j];
                            }
                        }
                        SendMessage(serverQueue, bufferArray[i], fileName, CommonMessage);
                    }
                    SendMessage(serverQueue, Encoding.ASCII.GetBytes($"{chunkCount}"),
                        fileName, LastMessage);
                } catch (Exception e) {
                    throw new InvalidOperationException(e.Message);
                } finally {
                    fileStream.Close();
                }
            }
            return true;
        }


        /// <summary>
        /// Send message to server
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="stream"></param>
        /// <param name="label"></param>
        /// <param name="appSpecific"></param>
        internal void SendMessage(MessageQueue queue, byte[] buf, string label, int appSpecific)
        {
            var message = CreateMessage(buf, label, appSpecific);

            queue.Send(message);
            message.Dispose();
        }

        internal Message CreateMessage(byte[] buf, string label, int appSpecific)
        {
            return new Message {
                BodyStream = new MemoryStream(buf),
                Label = label,
                Priority = MessagePriority.Normal,
                Formatter = new BinaryMessageFormatter(),
                AppSpecific = appSpecific
            };
        }
    }
}
