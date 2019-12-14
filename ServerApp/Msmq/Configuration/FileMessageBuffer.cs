using System.Collections.Generic;
using System.Linq;
using ServerApp.Msmq.Configuration.Interfaces;

namespace ServerApp.Msmq.Configuration
{
    public class FileMessageBuffer : IFileMessageBuffer
    {
        private IDictionary<string, FileMessage> _filesBuffer;
        private IServerOptions _options;

        public FileMessageBuffer(IServerOptions options)
        {
            _options = options;
            _filesBuffer = new Dictionary<string, FileMessage>();
        }
        
        public void AddFilePart(FilePart part)
        {
            var fileIdentifier = part.Part.Label;
            var isLastMultipleMessage = part.Part.AppSpecific == _options.MultipleMessageEndIdentificator;
            var type = part.Part.AppSpecific == _options.SingleMessageIdentificator
                ? MessageType.Single
                : MessageType.Multiple;

            if (_filesBuffer.TryGetValue(fileIdentifier, out FileMessage fileMessage))
            {
                if (isLastMultipleMessage) {
                    fileMessage.FileStatus = FileTransferPullState.ReadyToTransfer;
                } else
                    fileMessage.AddPart(part);
            } else {
                var msmqId = GetMessageId(part.Part.Id);
                var messageStatus = type == MessageType.Single
                    ? FileTransferPullState.ReadyToTransfer
                    : FileTransferPullState.InProgress;
                var fMessage = new FileMessage { FileName = fileIdentifier, ClientQueueId = msmqId, Type = type, FileStatus = messageStatus, IsCopied = false};

                fMessage.AddPart(part);
                _filesBuffer.Add(fileIdentifier, fMessage);
            }
        }

        public List<ReadyToCopyFiles> GetReadyFiles()
        {
            return _filesBuffer.Where(f => f.Value.FileStatus == FileTransferPullState.ReadyToTransfer)
                               .Select(k => new ReadyToCopyFiles { FileName = k.Value.FileName, FMessage = k.Value }).ToList();
        }

        public void RemoveFile(string fileIdentifier)
        {
            _filesBuffer.Remove(fileIdentifier);
        }

        private string GetMessageId(string fullId)
        {
            var index = fullId.IndexOf('\\');
            return fullId.Substring(0, index);
        } 
    }

    public class ReadyToCopyFiles
    {
        internal string FileName { get; set; }
        internal FileMessage FMessage { get; set; }

    }
}
