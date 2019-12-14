using System.Threading;

namespace ServerApp.Msmq
{
    public interface IServer
    {
        void Run(CancellationToken token);
    }
}
