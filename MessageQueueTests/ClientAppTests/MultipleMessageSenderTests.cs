using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClientApp.Configure.MessageSenders;
using Experimental.System.Messaging;
using Moq;
using NUnit.Framework;

namespace MessageQueueTests.ClientAppTests
{
    [TestFixture]
    public class MultipleMessageSenderTests
    {
        [Test]
        public void CreateMessageTest()
        {
            var label = "label";
            var sender = new MultipleMessageSender("blabla", 100);
            var message = sender.CreateMessage(new MemoryStream(), label, 1);

            Assert.IsNotNull(message.BodyStream);
            Assert.AreEqual(label, message.Label);
            Assert.AreEqual(MessagePriority.Normal, message.Priority);
            Assert.IsTrue(message.Formatter is BinaryMessageFormatter);
            Assert.AreEqual(1, message.AppSpecific);
        }

        [Test]
        public void SendFileTest()
        {
            var messageSenderMock = new Mock<MultipleMessageSender>("bla", 1);

            messageSenderMock.Setup(m => m.SendMessage(It.IsAny<MessageQueue>(), It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<int>())).Verifiable();
            
            var result = messageSenderMock.Object.SendFile("file", new MemoryStream());

            Assert.IsTrue(result);
        }

        [Test]
        public void SendFileExceptionTest()
        {
            var sender = new MultipleMessageSender("blabla", 1);

            Assert.Throws<InvalidOperationException>(
                () => sender.SendFile("", new MemoryStream()));

            Assert.Throws<InvalidOperationException>(
                () => sender.SendFile("bla", null));
        }
    }
}
