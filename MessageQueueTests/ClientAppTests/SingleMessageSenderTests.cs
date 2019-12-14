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
    public class SingleMessageSenderTests
    {
        [Test]
        public void CreateMessageTest()
        {
            //var label = "label";
            //var sender = new SingleMessageSender("blabla");
            //var message = sender.CreateMessage(label);

            //Assert.AreEqual(label, message.Label);
            //Assert.AreEqual(MessagePriority.Normal, message.Priority);
            //Assert.IsTrue(message.Formatter is BinaryMessageFormatter);
            //Assert.AreEqual(100, message.AppSpecific);
        }

        [Test]
        public void SendFileTest()
        {
            var messageSenderMock = new Mock<SingleMessageSender>("bla");

            //messageSenderMock.Setup(m => m.Send(It.IsAny<MessageQueue>(), It.IsAny<Message>())).Verifiable();
            //messageSenderMock.Setup(m => m.CreateMessage(It.IsAny<Stream>(), It.IsAny<string>())).Verifiable();

            //var result = messageSenderMock.Object.SendFile("file", new MemoryStream());

           // Assert.IsTrue(result);
        }

        [Test]
        public void SendFileExceptionTest()
        {
            //var sender = new SingleMessageSender("blabla");

            //Assert.Throws<InvalidOperationException>(
            //    () => sender.SendFile("", new MemoryStream()));

            //Assert.Throws<InvalidOperationException>(
            //    () => sender.SendFile("bla", null));
        }
    }
}
