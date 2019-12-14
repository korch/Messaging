using System.Collections.Generic;

namespace ServerApp.Msmq.Configuration
{
    public class FileMessage
    {
        public FileTransferPullState FileStatus { get; set; }
        public MessageType Type { get; set; }
        public string ClientQueueId { get; set; }
        public string FileName { get; set; }
        public bool IsCopied { get; set; }

        private List<FilePart> _parts = new List<FilePart>();

        public List<FilePart> Parts => _parts;

        public void AddPart(FilePart part)
        {
            _parts.Add(part);
        }
    }
}
