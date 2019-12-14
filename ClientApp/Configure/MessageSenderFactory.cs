using ClientApp.Configure.Interfaces;
using ClientApp.Configure.MessageSenders;

namespace ClientApp.Configure
{
    public class MessageSenderFactory : IMessageSenderFactory
    {
        public IMessageSender GetMessageSender(MessageType type, IMessageCreator messageCreator, IClientOptions options)
        {
            switch (type) {
                case MessageType.Single:
                    return new SingleMessageSender(messageCreator, options);
                case MessageType.Multiple:
                    return new MultipleMessageSender(messageCreator, options);
                default:
                    return new SingleMessageSender(messageCreator, options);
            }
        }
    }
}
