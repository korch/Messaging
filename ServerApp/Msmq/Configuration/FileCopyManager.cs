using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ServerApp.Msmq.Configuration.Interfaces;

namespace ServerApp.Msmq.Configuration
{
    public class FileCopyManager : IFileCopyManager
    {
        private readonly IServerOptions _options;
        private readonly object _locker = new object();

        public FileCopyManager(IServerOptions options)
        {
            _options = options;
        }

        public async Task CopyFiles(IFileMessageBuffer buffer)
        {
            var list = buffer.GetReadyFiles();
            
            if (list.Any()) {
                await Task.Run(() => {
                    lock (_locker) {
                        foreach (var file in list) {
                            var processor = GetMessageProcessorHelper.GetMessageProcessor(file.FMessage.Type, _options);
                            processor.Processing(file.FMessage.Parts);

                            buffer.RemoveFile(file.FileName);
                            Console.WriteLine($"File with name{file.FMessage.FileName} was copied to folder...");
                        }
                    }});
            }
        }
    }
}
