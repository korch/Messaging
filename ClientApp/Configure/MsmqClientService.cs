using ClientApp.Configure;
using System;
using System.IO;
using System.Threading;

namespace ClientApp
{
    public class MsmqClientService : IDisposable
    {
        private const string ServerQueueName = @".\Private$\MsmqTRansferFileQueue";    
        private string _queuePath = string.Empty;
        private bool _isDevOrQa;
        private Watcher _watcher;
        private ProcessingManager _manager;

        public MsmqClientService(bool DevOrQa = false)
        {
            _isDevOrQa = DevOrQa;
            _watcher = new Watcher(_isDevOrQa);
        }

        public void Run()
        {
            SetServerQueueName();
            _watcher.SetCreateHandler(OnChanged);
            _manager = new ProcessingManager(_queuePath);
        }

        private void SetServerQueueName()
        {
            if (!_isDevOrQa)
            {
                Console.WriteLine("Do you use a remote server queue? Y/N");
                var answer = Console.ReadLine();

                if (string.Equals(answer.ToLower(), "y"))
                {
                    Console.WriteLine(
                        "Please, write a full path for remote queue (pattern: FormatName:Direct=OS:machinename\\private$\\queuename)");
                    _queuePath = Console.ReadLine();

                    Console.WriteLine();
                    Console.WriteLine();

                    Console.WriteLine("Good! Now you uses a remote queue");
                }
                else
                {
                    _queuePath = ServerQueueName;
                }
            }
            else
                _queuePath = ServerQueueName;
        }

        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            Thread.Sleep(3000);
            var fileStream = new FileStream(e.FullPath, FileMode.Open);

            _manager.ProcessingFileSendingMessage(e.FullPath, fileStream);

            fileStream.Dispose();

            Console.WriteLine($"File: {e.FullPath} which was {e.ChangeType} was sent to Server.");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) {
                _watcher = null;
            }  
        }

        ~MsmqClientService()
        {
            Dispose(false);
        }
    }
}
