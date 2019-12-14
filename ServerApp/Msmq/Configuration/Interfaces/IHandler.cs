using Experimental.System.Messaging;

namespace ServerApp.Msmq.Configuration.Interfaces
{

    public interface IHandler
    {
        void Handle(Message message);
    }
}
