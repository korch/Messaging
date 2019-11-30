using System;
using System.IO;
using ClientApp.Configure;
using ClientApp.Configure.MessageSenders;
using Moq;
using NUnit.Framework;

namespace MessageQueueTests.ClientAppTests
{
    [TestFixture]
    public class ProcessingManagerTests
    {
        private Mock<ProcessingManager> _manager;

        [SetUp]
        public void Setup()
        {
            _manager = new Mock<ProcessingManager>() {CallBase = true};
        }

        [Test]
        public void GetMessageSenderTest()
        {
            var sender = _manager.Object.GetMessageSender(MessageType.Single);
            Assert.IsTrue(sender is SingleMessageSender);

            sender = _manager.Object.GetMessageSender(MessageType.Multiple);
            Assert.IsTrue(sender is MultipleMessageSender);
        }

        [Test]
        public void ProcessingFileSendingMessageTest()
        {
            _manager.Setup(m => m.GetMessageSender(It.IsAny<MessageType>())).Returns(new SingleMessageSender(""));
            _manager.Setup(m => m.SendMessage(It.IsAny<IMessageSender>(), It.IsAny<string>(), It.IsAny<Stream>())).Verifiable();

            var result = _manager.Object.ProcessingFileSendingMessage("file", new MemoryStream());

            Assert.IsTrue(result);
        }

        [Test]
        public void ProcessingFileSendingMessageExceptionTest()
        {
            Assert.Throws<InvalidOperationException>(
                () => _manager.Object.ProcessingFileSendingMessage("", new MemoryStream()));

            Assert.Throws<NullReferenceException>(
                () => _manager.Object.ProcessingFileSendingMessage("file", null));
        }

        [Test]
        public void SendMessageTest()
        {
            _manager.Setup(m => m.SendMessage(It.IsAny<IMessageSender>(), It.IsAny<string>(), It.IsAny<Stream>()));
            _manager.Object.SendMessage(new SingleMessageSender("blablabla"), "file", new MemoryStream());
            _manager.Verify(m => m.SendMessage(It.IsAny<IMessageSender>(), It.IsAny<string>(), It.IsAny<Stream>()), Times.Once);
        }
    }
}

