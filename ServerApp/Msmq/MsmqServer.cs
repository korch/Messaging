using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Experimental.System.Messaging;
using ServerApp.Msmq.Configuration;
using ServerApp.Msmq.Configuration.Interfaces;
using MessageType = ServerApp.Msmq.Configuration.MessageType;

namespace ServerApp.Msmq
{
    public class MsmqServer : IServer
    {
        private readonly IServerOptions _options;
        private readonly IFileSystemManager _fileManager;
        private readonly IQueueCreateManager _queueCreateManager;
        private readonly IMessageToFilePartMapper _messageToFilePartMapper;
        private readonly IFileCopyManager _copyManager;

        private readonly IFileMessageBuffer _filesBuffer;

        public MsmqServer(IServerOptions options, IFileSystemManager fileManager, IQueueCreateManager queueManager, IMessageToFilePartMapper messageToFilePartMapper, IFileMessageBuffer messageBuffer, IFileCopyManager copyManager)
        {
            _options = options;
            _fileManager = fileManager;
            _queueCreateManager = queueManager;
            _messageToFilePartMapper = messageToFilePartMapper;
            _filesBuffer = messageBuffer;
            _copyManager = copyManager;
        }

        public async void Run(CancellationToken token)
        {
            _fileManager.CreateDirectoryIfNotExists(_options.FolderToCopy);
            _queueCreateManager.CreateQueueIfNotExists(_options.MessageQueueName);

           if (_queueCreateManager.QueueExists(_options.MessageQueueName))
               Console.WriteLine($"Starting work.. waiting for files from the message queue...");
          
            await Task.Run(() =>Server(token), token);
        }

        private async Task Server(CancellationToken token)
        {
            using (var serverQueue = new MessageQueue(_options.MessageQueueName)) {
                serverQueue.Formatter = new BinaryMessageFormatter();
                serverQueue.MessageReadPropertyFilter.Body = true;
                serverQueue.MessageReadPropertyFilter.CorrelationId = true;
                serverQueue.MessageReadPropertyFilter.AppSpecific = true;

                while (true) {
                    var message = await Task.Factory.FromAsync(
                        serverQueue.BeginReceive(),
                        serverQueue.EndReceive);

                    if (token.IsCancellationRequested)
                        return;
                    
                    var filePart = _messageToFilePartMapper.MatToFilePart(message);
                    _filesBuffer.AddFilePart(filePart);

                    await Task.Run(() => _copyManager.CopyFiles(_filesBuffer), token);
                }
            }
        }
    }
}
