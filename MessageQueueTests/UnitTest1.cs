using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using ClientApp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using NUnit.Framework;
using ServerApp.Msmq;

namespace Tests
{
    public class Tests
    {
        private string _folderToCheck;
        private string _folderToCopy;
        private string _simplePdfFile;

        private MsmqService _serverService;
        private MsmqClientService _clientService;

        [SetUp]
        public void Setup()
        {
            _folderToCheck = $"C://DefaultFolder";
            _folderToCopy = $"C://DefaultServerFolder";
            _simplePdfFile = "Sample-PDF-File.pdf";

            if (!Directory.Exists(_folderToCheck)) {
                Directory.CreateDirectory(_folderToCheck);
            }

            if (!Directory.Exists(_folderToCopy)) {
                Directory.CreateDirectory(_folderToCopy);
            }

            _serverService = new MsmqService();
            _serverService.Run();

            _clientService = new MsmqClientService(DevOrQa:true);
            _clientService.Run();
        }

        [Test]
        public void SendSmallFileTest()
        {
            GenerateSimplePDF();
            //just to ensure that for these 5 seconds client app will find out a pdf file and send them to server via message queue.
            Thread.Sleep(5000);

            Assert.IsTrue(File.Exists($"{_folderToCopy}//Sample-PDF-File.pdf"));
        }


        private void GenerateSimplePDF()
        {
            FileStream fs = new FileStream($"{_folderToCheck}//Sample-PDF-File.pdf", FileMode.Create);

            // Create an instance of the document class which represents the PDF document itself.  
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            // Create an instance to the PDF file by creating an instance of the PDF   
            // Writer class using the document and the filestrem in the constructor.  

            PdfWriter writer = PdfWriter.GetInstance(document, fs);

            // Add meta information to the document  
            document.AddAuthor("Micke Blomquist");
            document.AddCreator("Sample application using iTextSharp");
            document.AddKeywords("PDF tutorial education");
            document.AddSubject("Document subject - Describing the steps creating a PDF document");
            document.AddTitle("The document title - PDF creation using iTextSharp");

            // Open the document to enable you to write to the document  
            document.Open();
            // Add a simple and wellknown phrase to the document in a flow layout manner  
            document.Add(new Paragraph("Hello World!"));
            // Close the document  
            document.Close();
            // Close the writer instance  
            writer.Close();
            // Always close open filehandles explicity  
            fs.Close();
        }

        [TearDown]
        public void Down()
        {
            if (File.Exists($"{_folderToCopy}//{_simplePdfFile}"))
                File.Delete($"{_folderToCopy}//{_simplePdfFile}");

            if (File.Exists($"{_folderToCheck}//{_simplePdfFile}"))
                File.Delete($"{_folderToCheck}//{_simplePdfFile}");

            if (Directory.Exists(_folderToCheck))
                Directory.Delete(_folderToCheck);

            if (Directory.Exists(_folderToCopy))
                Directory.Delete(_folderToCopy);
        }
    }
}