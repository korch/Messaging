using System;
using System.Collections.Generic;
using System.Text;
using ClientApp.Configure.Interfaces;

namespace ClientApp.Configure
{
    public class ClientOptionsProvider : IClientOptionsProvider
    {
        private IClientOptions _options;
        public ClientOptionsProvider(IClientOptions options)
        {
            _options = options;
        }

        public IClientOptions GetOptions()
        {
            return _options;
        }
    }
}
