
namespace ServerApp.Msmq.Configuration
{
    public enum MessageType
    {
        Single = 1,
        Multiple = 2
    }

    public class GetMessageProcessorHelper
    {
        public static Processor GetMessageProcessor(MessageType type)
        {
            switch (type) {
                case MessageType.Single:
                    return new SingleMessageFileProcessor();
                case MessageType.Multiple:
                    return new MultipleMessageFileProcessor();
                default:
                    return new SingleMessageFileProcessor();
            }
        }
    }
}
