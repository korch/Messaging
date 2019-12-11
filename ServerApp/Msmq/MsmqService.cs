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
using MessageType = ServerApp.Msmq.Configuration.MessageType;

namespace ServerApp.Msmq
{
    public class FileMessage
    {
        private FileStatus = 

        private List<FilePart> _parts;

        public void AddPart(FilePart part)
        {
            _parts.Add(part);
        }
    }

    public class FileMessageBuffer
    {
        private IDictionary<string, FileMessage> _filesBuffer;

    

        public void AddFilePart(string fileIdentifier, FilePart part)
        {
            if (_filesBuffer.TryGetValue(fileIdentifier, out FileMessage fileMessage))
            {
                fileMessage.AddPart(part);
            }

            _filesBuffer.Add(fileIdentifier, part);
        }

        public void RemoveFile(string fileIdentifier)
        {

        }
    }


    public  class MsmqServiceFactory
    {
        private IServerOptionsProvider _optionsProvider;

        public MsmqServiceFactory(IServerOptionsProvider optionsProvider)
        {
            _optionsProvider = optionsProvider;

        }

        public IServer GetServer ()
        {
            return new MsmqService(_optionsProvider.GetOptions());
        }
    }


    public class MsmqServiceOptions
    {
        public string MessageQueueName { get; set; }

        // Other settings
    }

    public interface IFileSystemManager
    {
        void CreateDirectoryIfNotExists();
    }

    public class FileSystemManager : IFileSystemManager
    {
        public FileSystemManager(IDirectory directory)
        {

        }

        public void CreateDirectoryIfNotExists()
        {
            if (!_directory.Exists())
            {
                _directory.Create();
            }
        }
    }

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

        public MsmqService(MsmqServiceOptions options)
        {

        }

        public async void Run()
        {
            ReadAppSettings();
            CreateQueue();

            if (!Directory.Exists(DefaultPath))
                Directory.CreateDirectory(DefaultPath);

          
            _filesToCopy = new List<FileTransferPull>();
            
         
            await Task.Run(Server);
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

        public class MsmqHandler : IHandler
        {
            public void Handle(MsmqMessage message)
            {

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

                    Task.Run()

                    var filePart = messageToFilePartMapper.MatToFilePart(message);

                    _filesBuffer.AddPart(filePart);

                    //obtain messages
                    MessageProcess(message);
                }
            }
        }

        private void CopyFiles()
        {
             Task.Run(() => {
                 lock (locker) {
                     if (_filesToCopy.Any(f => f.State == FileTransferPullState.ReadyToTransfer)) {
                         var files = _filesToCopy.Where(f => f.State == FileTransferPullState.ReadyToTransfer);
                         foreach (var file in files) {
                             var processor = GetMessageProcessorHelper.GetMessageProcessor(file.Type);
                             processor.Processing(file.Messages);
                             file.IsCopied = true;

                             Console.WriteLine(
                                 $"New file with name {file.FileName} from client queue with Id: {file.ClientQueueId} was copied to folder...");
                         }
                         _filesToCopy = _filesToCopy.Where(f => !f.IsCopied).ToList();
                     }
                 }
            });
        }

        private void MessageProcess(Message message)
        {
            lock (locker) {
                if (message.AppSpecific == _singleMessageIdentificator) {
                    var file = CreateFileTransferObject(true, message);

                    file.Messages.Add(message);
                    _filesToCopy.Add(file);
                }

                if (message.AppSpecific == _multipleMessageStartIdentificator) {
                    var file = CreateFileTransferObject(false, message);
                    _filesToCopy.Add(file);
                }
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

            CopyFiles();
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
            ServerQueueName = 
                ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location).AppSettings.Settings["MessageQueueName"].Value;
            DefaultPath =
                ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location).AppSettings.Settings["FolderToCopy"].Value;
            _singleMessageIdentificator = 
                int.Parse(ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location).AppSettings.Settings["SingleMessageIdentificator"].Value);
            _multipleMessageStartIdentificator = 
                int.Parse(ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location).AppSettings.Settings["MultipleMessageStartIdentificator"].Value);
            _multipleMessageEndIdentificator = 
                int.Parse(ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location).AppSettings.Settings["MultipleMessageEndIdentificator"].Value);
            _multipleCommonMessageIdentificator =
                int.Parse(ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location).AppSettings.Settings["MultipleCommonMessageIdentificator"].Value);
        }
    }
}
