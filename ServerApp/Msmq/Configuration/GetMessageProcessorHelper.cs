
using ServerApp.Msmq.Configuration.Interfaces;

namespace ServerApp.Msmq.Configuration
{
    public enum MessageType
    {
        Single = 1,
        Multiple = 2
    }

    public class GetMessageProcessorHelper
    {
        public static Processor GetMessageProcessor(MessageType type, IServerOptions options)
        {
            switch (type) {
                case MessageType.Single:
                    return new SingleMessageFileProcessor(options);
                case MessageType.Multiple:
                    return new MultipleMessageFileProcessor(options);
                default:
                    return new SingleMessageFileProcessor(options);
            }
        }
    }
}
