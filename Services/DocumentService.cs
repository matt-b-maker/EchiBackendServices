using EchiBackendServices.Models;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using System.Text.RegularExpressions;
using Xceed.Document.NET;
using Xceed.Words.NET;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Color = System.Drawing.Color;
using Container = Xceed.Document.NET.Container;
using Formatting = Xceed.Document.NET.Formatting;
using Image = SixLabors.ImageSharp.Image;

namespace EchiBackendServices.Services;

public class DocumentService(AzureBlobStorageService azureBlobStorageService)
{
    public string DocumentsDirectory { get; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Documents");

    public string InspectionAgreementFilePath { get; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Inspection Agreement.docx");

    public string RadonAddendumFilePath { get; set; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Radon Addendum.docx");

    public string InspectionReportFilePath { get; set; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Inspection Report.docx");

    private static Dictionary<string, string> _replacePatterns;


    //TODO for inspection report
    public List<Paragraph> ConclusoryStatementRunsList { get; set; } = new();

    public void AddRunToList(List<Paragraph> paragraphs, string title, bool period = false, Color? color = null)
    {
        return;
    }

    public async Task<string> CreateInspectionAgreement(ClientModel client)
    {
        try
        {
            Directory.CreateDirectory(DocumentsDirectory); // Create directory if it doesn't exist

            // Read the embedded resource stream for DOCX and Save the embedded DOCX resource to the file system
            const string inspectionAgreementTemplateResourceName =
                "EchiBackendServices.Resources.Documents.Inspection Agreement Template.docx";
            await using var inspectionAgreementTemplateStream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(inspectionAgreementTemplateResourceName);
            if (File.Exists(InspectionAgreementFilePath)) File.Delete(InspectionAgreementFilePath);
            await using var inspectionAgreementTemplateFileStream = File.Create(InspectionAgreementFilePath);
            await inspectionAgreementTemplateStream?.CopyToAsync(inspectionAgreementTemplateFileStream)!;
            inspectionAgreementTemplateStream.Close();
            inspectionAgreementTemplateFileStream.Close();

            // Replace the placeholder text with the current date   
            var currentDate = DateTime.Today.ToString("M/d/yyyy");

            _replacePatterns = new Dictionary<string, string>()
            {
                {"This agreement dated _____.", $"This agreement dated {currentDate}."},
                {"{today’s date}", $" {currentDate}"},
                {"Client: __________________", $"Client: {client.ClientFirstName} {client.ClientLastName}"},
                {"{name}", $"{client.ClientFirstName} {client.ClientLastName}"},
                {"(Address 1}", client.ClientAddressLineOne},
                {"{address 2}", client.ClientAddressLineTwo},
                {"{City, state zip}", $"{client.ClientAddressCity}, {client.ClientAddressState} {client.ClientAddressZipCode}"},
                {"{phone}", client.ClientPhoneNumber},
                {"{Email}", client.ClientEmailAddress},
                {"{Inspection address 1}", client.InspectionAddressLineOne},
                {"{Inspection address 2}", client.InspectionAddressLineTwo},
                {"{inspection city, state zip}", $"{client.InspectionAddressCity}, {client.InspectionAddressState} {client.InspectionAddressZipCode}"},
                {"{Inspection Fee}", client.Fee}
            };

            // Load the DOCX document using Xceed DocX
            using var inspectionAgreementDocument = DocX.Load(InspectionAgreementFilePath);

            inspectionAgreementDocument.ReplaceText("This agreement dated ______", $"This agreement dated {currentDate}");
            inspectionAgreementDocument.ReplaceText("{today’s date}", $" {currentDate}");
            inspectionAgreementDocument.ReplaceText("Client: __________________", $"Client: {client.ClientFirstName} {client.ClientLastName}");
            if (!string.IsNullOrEmpty(client.ClientFirstName) || !string.IsNullOrEmpty(client.ClientLastName))
                inspectionAgreementDocument.ReplaceText("{name}", $"{client.ClientFirstName} {client.ClientLastName}");
            if (!string.IsNullOrEmpty(client.ClientAddressLineOne))
                inspectionAgreementDocument.ReplaceText("(Address 1}", client.ClientAddressLineOne);
            if (!string.IsNullOrEmpty(client.ClientAddressLineTwo))
                inspectionAgreementDocument.ReplaceText("{address 2}", client.ClientAddressLineTwo);
            if (!string.IsNullOrEmpty(client.ClientAddressCity) ||
                !string.IsNullOrEmpty(client.ClientAddressState) ||
                !string.IsNullOrEmpty(client.ClientAddressZipCode))
                inspectionAgreementDocument.ReplaceText("{City, state zip}",
                    $"{client.ClientAddressCity}, {client.ClientAddressState} {client.ClientAddressZipCode}");
            if (!string.IsNullOrEmpty(client.ClientPhoneNumber))
                inspectionAgreementDocument.ReplaceText("{phone}", client.ClientPhoneNumber);
            if (!string.IsNullOrEmpty(client.ClientEmailAddress))
                inspectionAgreementDocument.ReplaceText("{Email}", client.ClientEmailAddress);

            if (!string.IsNullOrEmpty(client.InspectionAddressLineOne))
                inspectionAgreementDocument.ReplaceText("{Inspection address 1}", client.InspectionAddressLineOne);
            if (!string.IsNullOrEmpty(client.InspectionAddressLineTwo))
                inspectionAgreementDocument.ReplaceText("{Inspection address 2}", client.InspectionAddressLineTwo);
            if (!string.IsNullOrEmpty(client.InspectionAddressCity) ||
                !string.IsNullOrEmpty(client.InspectionAddressState) ||
                !string.IsNullOrEmpty(client.InspectionAddressZipCode))
                inspectionAgreementDocument.ReplaceText("{inspection city, state zip}",
                    $"{client.InspectionAddressCity}, {client.InspectionAddressState} {client.InspectionAddressZipCode}");

            if (!string.IsNullOrEmpty(client.Fee)) inspectionAgreementDocument.ReplaceText("{Inspection Fee}", $"${client.Fee.Trim('$')}");

            //if (inspectionAgreementDocument.Text.Contains("{inspection city, state zip}"))
            //{
            //    Console.WriteLine("");
            //}

            var stringsToRemove = GetListOfStringsToRemove(client);
            var strToRemoves = stringsToRemove as string[] ?? stringsToRemove.ToArray();
            if (strToRemoves.Any())
            {
                //example of removing a paragraph
                for (var i = inspectionAgreementDocument.Paragraphs.Count - 1; i >= 0; i--)
                {
                    var paragraph = inspectionAgreementDocument.Paragraphs[i];
                    foreach (var strToRemove in strToRemoves)
                    {
                        if (paragraph.Text.Contains(strToRemove))
                        {
                            paragraph.Remove(false);
                            break; // No need to continue checking once a match is found
                        }
                    }
                }
            }

            //// Do the replacement of all the found tags and with green bold strings.
            //var replaceTextOptions = new FunctionReplaceTextOptions()
            //{
            //    FindPattern = "<(.*?)>",
            //    RegexMatchHandler = ReplaceFunc,
            //    RegExOptions = RegexOptions.IgnoreCase,
            //    NewFormatting = new Formatting() { Bold = true, FontColor = System.Drawing.Color.Green }
            //};
            //inspectionAgreementDocument.ReplaceText(replaceTextOptions);

            //await AddImageToDocumentAsync(inspectionAgreementDocument, @"https://echifilestorage.blob.core.windows.net/echiphotos/Bathroom 1 b639554a-c715-4084-91d6-8079fb78925a");

            inspectionAgreementDocument.InsertParagraph("\n\n\n\n\n\n\n");

            inspectionAgreementDocument.Save();

            // Get the stream from the file path
            await using var fileStream = new FileStream(InspectionAgreementFilePath, FileMode.Open, FileAccess.Read);

            // Pass the stream to your method
            return await azureBlobStorageService.UploadFileToAzureStorage(fileStream, "echidocs",
                $"{client.ClientFirstName} {client.ClientLastName} {client.InspectionAddressLineOne} Inspection Agreement {Guid.NewGuid()}.docx");
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    public async Task<string> CreateRadonAddendum(ClientModel client)
    {
        const string radonAddendumTemplateResourceName =
            "EchiBackendServices.Resources.Documents.Radon Addendum Template.docx";

        Directory.CreateDirectory(DocumentsDirectory); // Create directory if it doesn't exist

        // Save the embedded DOCX resource to the file system

        // Read the embedded resource stream for DOCX
        await using var radonAddendumTemplateStream =
            Assembly.GetExecutingAssembly().GetManifestResourceStream(radonAddendumTemplateResourceName);
        if (File.Exists(RadonAddendumFilePath)) File.Delete(RadonAddendumFilePath);
        await using var radonAddendumTemplateFileStream = File.Create(RadonAddendumFilePath);
        await radonAddendumTemplateStream?.CopyToAsync(radonAddendumTemplateFileStream)!;
        radonAddendumTemplateStream.Close();
        radonAddendumTemplateFileStream.Close();

        // Replace the placeholder text with the current date
        var currentDate = DateTime.Today.ToString("M/d/yyyy");

        //// Load the DOCX document using Xceed DocX
        using var radonDocument = DocX.Load(RadonAddendumFilePath);
        //using var inspectionAgreementDocument = DocX.Load(InspectionAgreementFilePath);

        radonDocument.ReplaceText("dated _____________", $"dated {currentDate}");
        radonDocument.ReplaceText("$________", $"{client.RadonFee}");
        radonDocument.ReplaceText("Client:", $"Client: {client.ClientFirstName} {client.ClientLastName}");

        //find the second paragraph with instance of Dated: and replace it with the current date
        var datedParagraph = radonDocument.Paragraphs.LastOrDefault(p => p.Text.Contains("Dated:"));

        if (datedParagraph != null)
        {
            datedParagraph.ReplaceText("Dated:", $"Dated: {currentDate}");
        }

        radonDocument.Save();

        // Get the stream from the file path
        await using var fileStream = new FileStream(RadonAddendumFilePath, FileMode.Open, FileAccess.Read);

        // Pass the stream to your method
        return await azureBlobStorageService.UploadFileToAzureStorage(fileStream, "echidocs",
            $"{client.ClientFirstName} {client.ClientLastName} {client.InspectionAddressLineOne} Radon Addendum {Guid.NewGuid()}.docx");
    }

    public async Task<string> CreateInspectionReport(ClientModel client, List<DocumentTextLineModel> inspectionReportLines)
    {
        Directory.CreateDirectory(DocumentsDirectory); // Create directory if it doesn't exist

        // Read the embedded resource stream for DOCX and Save the embedded DOCX resource to the file system
        const string inspectionReportTemplateResourceName =
            "EchiBackendServices.Resources.Documents.Inspection Report Template.docx";
        await using var inspectionReportTemplateStream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(inspectionReportTemplateResourceName);
        if (File.Exists(InspectionReportFilePath)) File.Delete(InspectionReportFilePath);
        await using var inspectionReportTemplateFileStream = File.Create(InspectionReportFilePath);
        await inspectionReportTemplateStream?.CopyToAsync(inspectionReportTemplateFileStream)!;
        inspectionReportTemplateStream.Close();
        inspectionReportTemplateFileStream.Close();

        // Load the DOCX document using Xceed DocX
        using var inspectionReportDocument = DocX.Load(InspectionReportFilePath);

        //Fill in client info
        inspectionReportDocument.ReplaceText("{Name}", $"{client.ClientFirstName} {client.ClientLastName}");
        inspectionReportDocument.ReplaceText("{ClientAddressLine1}", $"{client.ClientAddressLineOne}");
        inspectionReportDocument.ReplaceText("{ClientAddressLine2}",
            !string.IsNullOrEmpty(client.ClientAddressLineTwo) ? $"{client.ClientAddressLineTwo}" : string.Empty);
        inspectionReportDocument.ReplaceText("{Client City State ZIP}", $"{client.ClientAddressCity}, {client.ClientAddressState} {client.ClientAddressZipCode}");
        inspectionReportDocument.ReplaceText("{Client Phone}", $"{client.ClientPhoneNumber}");
        inspectionReportDocument.ReplaceText("{Client Email}", $"{client.ClientEmailAddress}");
        // replace {Today’s Date} with today's date
        inspectionReportDocument.ReplaceText("{Today’s Date}", $"{DateTime.Today:MM/dd/yyyy}");
        
        //fill in inspection address info
        inspectionReportDocument.ReplaceText("{InspectionAddressLine1}", $"{client.InspectionAddressLineOne}");
        inspectionReportDocument.ReplaceText("{InspectionAddressLine2}",
                       !string.IsNullOrEmpty(client.InspectionAddressLineTwo) ? $"{client.InspectionAddressLineTwo}" : string.Empty);
        inspectionReportDocument.ReplaceText("{Inspection City State ZIP}", $"{client.InspectionAddressCity}, {client.InspectionAddressState} {client.InspectionAddressZipCode}");

        inspectionReportDocument.ReplaceText("{Selling Agent}", $"{client.AgentName}");

        //replace text {inspection image} with image from url
        var inspectionImageParagraph = inspectionReportDocument.Paragraphs.FirstOrDefault(p => p.Text.Contains("{inspection image}"));
        if (inspectionImageParagraph != null)
        {
            var imageUrl = @"https://echifilestorage.blob.core.windows.net/echiphotos/Bathroom 1 b639554a-c715-4084-91d6-8079fb78925a";
            await AddImageToDocumentAsync(inspectionReportDocument, imageUrl, inspectionReportDocument.Paragraphs.IndexOf(inspectionImageParagraph), 0.9f, true);
            inspectionImageParagraph.Remove(false);
        }

        var groundsLines = inspectionReportLines.Where(l => l.SectionName == InspectionSections.GroundsSection).ToList();

        var groundsParagraph = inspectionReportDocument.Paragraphs.FirstOrDefault(p => p.Text.Contains(InspectionSections.GroundsSection));

        if (groundsParagraph != null)
        {
            inspectionReportDocument.ReplaceText("{grading}", string.Empty);
            foreach (var line in groundsLines)
            {
                groundsParagraph.InsertText(line.LineText, false, new Formatting() { Bold = true, FontColor = line.Color });
                groundsParagraph.InsertText("\n", false, new Formatting() { Bold = true, FontColor = line.Color });
            }
        }

        inspectionReportDocument.Save();

        // Get the stream from the file path
        await using var fileStream = new FileStream(InspectionReportFilePath, FileMode.Open, FileAccess.Read);

        // Pass the stream to your method
        return await azureBlobStorageService.UploadFileToAzureStorage(fileStream, "echidocs",
            $"{client.ClientFirstName} {client.ClientLastName} {client.InspectionAddressLineOne} Inspection Report {Guid.NewGuid()}.docx");
    }

    private static string ReplaceFunc(string findStr)
    {
        return _replacePatterns.GetValueOrDefault(findStr, findStr);
    }

    private async Task AddImageToDocumentAsync(Document inspectionAgreementDocument, string imageUrl, int paragraphIndex, float imageWidth = 0.5f, bool centerImage = false)
    {
        try
        {
            // Download the image bytes from the web URL
            using var httpClient = new HttpClient();
            var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);

            // Create a MemoryStream from the downloaded image bytes
            using var imageStream = new MemoryStream(imageBytes);

            // Use ImageSharp to get the image dimensions
            using var image = Image.Load(imageBytes);

            // Calculate the desired width based on half of the page width
            // Adjust this calculation as needed based on your document layout
            var pageWidth = inspectionAgreementDocument.PageWidth;
            var desiredWidth = pageWidth * imageWidth; // Set the image width to half the page width

            // Calculate the scaled height to maintain aspect ratio
            var aspectRatio = image.Width / (float)image.Height;
            var desiredHeight = (int)(desiredWidth / aspectRatio);

            // Resize the image using ImageSharp
            image.Mutate(x => x.Resize((int)desiredWidth, desiredHeight));

            // Convert the resized image back to bytes
            using var resizedImageStream = new MemoryStream();
            image.SaveAsJpeg(resizedImageStream); // Save as JPEG format or other desired format

            // Add the resized image stream to the document
            var newParagraph = inspectionAgreementDocument.InsertParagraph(paragraphIndex, "", false);
            var resizedImage = inspectionAgreementDocument.AddImage(resizedImageStream);
            var picture = resizedImage.CreatePicture();
            newParagraph.AppendPicture(picture);

            // Optionally center the image
            if (centerImage)
            {
                newParagraph.Alignment = Alignment.center;
            }
        }
        catch (Exception ex)
        {
            // Handle or log the exception
            Console.WriteLine($"Error adding image to document: {ex.Message}");
            throw; // Optionally rethrow the exception
        }
    }

    private void ReplaceLastDatedInInspectionAgreement(Container doc, string currentDate)
    {
        // Iterate through paragraphs in reverse order
        for (var i = doc.Paragraphs.Count - 1; i >= 0; i--)
        {
            var paragraph = doc.Paragraphs[i];
            var lines = paragraph.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            for (var j = lines.Length - 1; j >= 0; j--)
            {
                var line = lines[j];
                var lastIndex = line.LastIndexOf("Dated:", StringComparison.Ordinal);
                if (lastIndex == -1) continue;
                // Replace only the last occurrence of "Dated:" in the line
                var updatedLine = line[..lastIndex] + $"Dated: {currentDate}" + line[(lastIndex + "Dated:".Length)..];
                paragraph.ReplaceText(line, updatedLine);
                return; // Exit the loop after replacing the text in the last occurrence
            }
        }
    }

    private IEnumerable<string> GetListOfStringsToRemove(ClientModel client)
    {
        var listOfStrings = new List<string>();
        if (string.IsNullOrEmpty(client.ClientFirstName) && string.IsNullOrEmpty(client.ClientLastName))
            listOfStrings.Add("{name}");
        if (string.IsNullOrEmpty(client.ClientAddressLineOne)) listOfStrings.Add("(Address 1}");
        if (string.IsNullOrEmpty(client.ClientAddressLineTwo)) listOfStrings.Add("{address 2}");
        if (string.IsNullOrEmpty(client.ClientAddressCity) || string.IsNullOrEmpty(client.ClientAddressState) || string.IsNullOrEmpty(client.ClientAddressZipCode)) listOfStrings.Add("{City, state zip}");
        if (string.IsNullOrEmpty(client.ClientPhoneNumber)) listOfStrings.Add("{phone}");
        if (string.IsNullOrEmpty(client.ClientEmailAddress)) listOfStrings.Add("{Email}");
        if (string.IsNullOrEmpty(client.InspectionAddressLineOne)) listOfStrings.Add("{Inspection address 1}");
        if (string.IsNullOrEmpty(client.InspectionAddressLineTwo)) listOfStrings.Add("{Inspection address 2}");
        if (string.IsNullOrEmpty(client.InspectionAddressCity) || string.IsNullOrEmpty(client.InspectionAddressState) || string.IsNullOrEmpty(client.InspectionAddressZipCode)) listOfStrings.Add("{inspection city, state zip}");
        if (string.IsNullOrEmpty(client.Fee)) listOfStrings.Add("{Inspection Fee}");

        return listOfStrings;
    }
}