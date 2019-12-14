using ClientApp.Configure.MessageSenders;

namespace ClientApp.Configure.Interfaces
{
    public interface IMessageSenderFactory
    {
        IMessageSender GetMessageSender(MessageType type, IMessageCreator messageCreator, IClientOptions options);
    }
}
