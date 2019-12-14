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
            //var sender = _manager.Object.GetMessageSender(MessageType.Single);
            //Assert.IsTrue(sender is SingleMessageSender);

            //sender = _manager.Object.GetMessageSender(MessageType.Multiple);
            //Assert.IsTrue(sender is MultipleMessageSender);
        }

        [Test]
        public void ProcessingFileSendingMessageExceptionTest()
        { 
            Assert.Throws<InvalidOperationException>(
                () => _manager.Object.ProcessingFileSendingMessage(""));
            Assert.Throws<InvalidOperationException>(
                () => _manager.Object.ProcessingFileSendingMessage(null));
        }
    }
}

