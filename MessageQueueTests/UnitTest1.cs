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
    /// <summary>
    /// Important. I don't know why it happened, but if you want to run tests, you should run these test separately. Don't you Run All Tests button...
    /// </summary>
    public class Tests
    {
        private string _folderToCheck;
        private string _folderToCopy;
        private string _pdfFIleName;

        private MsmqService _serverService;
        private MsmqClientService _clientService;

        [SetUp]
        public void Setup()
        {
            _folderToCheck = $"C://DefaultFolder";
            _folderToCopy = $"C://DefaultServerFolder";
            _pdfFIleName = "Sample-PDF-File.pdf";

            if (!Directory.Exists(_folderToCheck))
            {
                Directory.CreateDirectory(_folderToCheck);
            }

            if (!Directory.Exists(_folderToCopy))
            {
                Directory.CreateDirectory(_folderToCopy);
            }

            _serverService = new MsmqService();
            _serverService.Run();

            _clientService = new MsmqClientService(DevOrQa: true);
            _clientService.Run();
        }

        [Test]
        public void SendSmallFileTest()
        {
            GeneratePDF();
            //just to ensure that for these 5 seconds client app will find out a pdf file and send them to server via message queue.
            Thread.Sleep(5000);

            Assert.IsTrue(File.Exists($"{_folderToCopy}//{_pdfFIleName}"));
        }

        [Test]
        public void SendBigFileTest()
        {
            CreateBigFile();

            //just to ensure that for these 5 seconds client app will find out a pdf file and send them to server via message queue.
            Thread.Sleep(5000);

            Assert.IsTrue(File.Exists($"{_folderToCopy}//{_pdfFIleName}"));
        }

        private void CreateBigFile()
        {
            File.Copy("Sample-PDF-File.pdf", $"{_folderToCheck}//{_pdfFIleName}");
        }

        //Why not do it like with big pdf file ? Just wanted to use something features to create a pdf file in C#.
        private void GeneratePDF()
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
            if (File.Exists($"{_folderToCopy}//{_pdfFIleName}"))
                File.Delete($"{_folderToCopy}//{_pdfFIleName}");

            if (File.Exists($"{_folderToCheck}//{_pdfFIleName}"))
                File.Delete($"{_folderToCheck}//{_pdfFIleName}");

            if (Directory.Exists(_folderToCheck))
                Directory.Delete(_folderToCheck);

            if (Directory.Exists(_folderToCopy))
                Directory.Delete(_folderToCopy);
        }
    }
}