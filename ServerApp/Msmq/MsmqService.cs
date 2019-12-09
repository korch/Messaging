using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Experimental.System.Messaging;
using ServerApp.Msmq.Configuration;
using MessageType = ServerApp.Msmq.Configuration.MessageType;

namespace ServerApp.Msmq
{
    public class MsmqService : IServer
    {
        private string ServerQueueName;
        private string DefaultPath;

        private List<FileTransferPull> _filesToCopy;

        private int _singleMessageIdentificator;
        private int _multipleMessageStartIdentificator;
        private int _multipleCommonMessageIdentificator;
        private int _multipleMessageEndIdentificator;

        private readonly object locker = new object();

        public void Run()
        {
            ReadAppSettings();
            CreateQueue();

            if (!Directory.Exists(DefaultPath))
                Directory.CreateDirectory(DefaultPath);

            _filesToCopy = new List<FileTransferPull>();
         
            Task.Run(Server);
        }

        private void CreateQueue()
        {
            if (!MessageQueue.Exists(ServerQueueName)) {
                MessageQueue.Create(ServerQueueName);
                Console.WriteLine($"MSMQ queue was created with name:{ServerQueueName}");
            } else {
                Console.WriteLine($"You uses MSMQ with name:{ServerQueueName}");      
            }
        }

        private async Task Server()
        {
            using (var serverQueue = new MessageQueue(ServerQueueName)) {
                serverQueue.Formatter = new BinaryMessageFormatter();
                serverQueue.MessageReadPropertyFilter.Body = true;
                serverQueue.MessageReadPropertyFilter.CorrelationId = true;
                serverQueue.MessageReadPropertyFilter.AppSpecific = true;

                while (true) {
                    var message = await Task.Factory.FromAsync(
                        serverQueue.BeginReceive(),
                        serverQueue.EndReceive);

                    //obtain messages
                    await MessageProcess(message);
                }
            }
        }

        private async Task CopyFiles()
        {
            await Task.Run(() => {
                if (_filesToCopy.Any(f => f.State == FileTransferPullState.ReadyToTransfer)) {
                    lock (locker) {
                        var files = _filesToCopy.Where(f => f.State == FileTransferPullState.ReadyToTransfer);
                        foreach (var file in files) {
                            var processor = GetMessageProcessorHelper.GetMessageProcessor(file.Type);
                            processor.Processing(file.Messages);
                            file.IsCopied = true;

                            Console.WriteLine($"New file with name {file.FileName} was copied to folder...");
                        }
                        _filesToCopy = _filesToCopy.Where(f => !f.IsCopied).ToList();
                    }
                }
            });
        }

        private async Task MessageProcess(Message message)
        {
            if (message.AppSpecific == _singleMessageIdentificator)
            {
                var file = CreateFileTransferObject(true, message);

                file.Messages.Add(message);
                _filesToCopy.Add(file);
            }

            if (message.AppSpecific == _multipleMessageStartIdentificator)
            {
                var file = CreateFileTransferObject(false, message);
                _filesToCopy.Add(file);
            }

            if (message.AppSpecific == _multipleCommonMessageIdentificator)
            {
                var ftc = GetFileTransferObject(message);

                if (ftc?.State == FileTransferPullState.ReadyToTransfer)
                    ftc.State = FileTransferPullState.InProgress;

                ftc?.Messages.Add(message);
            }

            if (message.AppSpecific == _multipleMessageEndIdentificator) {
                var ftc = GetFileTransferObject(message);
             
                if (ftc != null)
                    ftc.State = FileTransferPullState.ReadyToTransfer;
            }

            await CopyFiles();
        }

        private FileTransferPull CreateFileTransferObject(bool isSingle, Message message)
        {
            var index = message.Id.IndexOf('\\');
            var state = isSingle ? FileTransferPullState.ReadyToTransfer : FileTransferPullState.InProgress;
            var type = isSingle ? MessageType.Single : MessageType.Multiple;

            var fileToCopy = new FileTransferPull {
                ClientQueueId = message.Id.Substring(0, index),
                FileName = message.Label,
                State = state,
                Type = type,
                IsCopied = false
            };

            return fileToCopy;
        }

        private FileTransferPull GetFileTransferObject(Message message)
        {
            var index = message.Id.IndexOf('\\');
            var id = message.Id.Substring(0, index);
            var ftc = _filesToCopy.FirstOrDefault(f => string.Equals(f.ClientQueueId, id));

            return ftc;
        }

        private void ReadAppSettings()
        {
            ServerQueueName = ConfigurationManager.AppSettings["MessageQueueName"];
            DefaultPath = ConfigurationManager.AppSettings["FolderToCopy"];
            _singleMessageIdentificator = int.Parse(ConfigurationManager.AppSettings["SingleMessageIdentificator"]);
            _multipleMessageStartIdentificator = int.Parse(ConfigurationManager.AppSettings["MultipleMessageStartIdentificator"]);
            _multipleMessageEndIdentificator = int.Parse(ConfigurationManager.AppSettings["MultipleMessageEndIdentificator"]);
            _multipleCommonMessageIdentificator = int.Parse(ConfigurationManager.AppSettings["MultipleCommonMessageIdentificator"]);
        }
    }
}
