using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Experimental.System.Messaging;

namespace ClientApp.Configure.MessageSenders
{
    public class SingleMessageSender : IMessageSender
    {
        private readonly string _queueName;

        // for identifying that this is a single message
        private const int AppSpecific = 100;

        public SingleMessageSender(string messageQueuePath)
        {
            _queueName = messageQueuePath;
        }

        /// <summary>
        /// Process single message. When we shouldn't split a byte array for chunks
        /// </summary>
        /// <param name="fullPath"></param>
        public bool SendFile(string path)
        {
            using (var serverQueue = new MessageQueue(_queueName, QueueAccessMode.Send)) {
                var fileStream = new FileStream(path, FileMode.Open);
                try {
                    var message = CreateMessage(Path.GetFileName(path));

                    message.BodyStream = fileStream;
                    Send(serverQueue, message);

                    message.Dispose();
                } catch (Exception e) {
                    throw new InvalidOperationException(e.Message);
                } finally {
                    fileStream.Close();
                }
            }
            return true;
        }

        private void Send(MessageQueue queue, Message message)
        {
            queue.Send(message);
        }

        private Message CreateMessage(string label)
        {
            return new Message {
                Label = label,
                Priority = MessagePriority.Normal,
                Formatter = new BinaryMessageFormatter(),
                AppSpecific = AppSpecific
            };
        }
    }
}
