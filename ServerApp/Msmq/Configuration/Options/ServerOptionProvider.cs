using ServerApp.Msmq.Configuration.Interfaces;

namespace ServerApp.Msmq.Configuration.Options
{
    public class ServerOptionsProvider : IServerOptionsProvider
    {
        private IServerOptions _options;
        public ServerOptionsProvider(IServerOptions options)
        {
            _options = options;
        }

        public IServerOptions GetOptions()
        {
            return _options;
        }
    }
}
