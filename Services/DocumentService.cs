using EchiBackendServices.Models;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
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
            await using var inspectionAgreementTemplateFileStream = File.Create(InspectionAgreementFilePath);
            await inspectionAgreementTemplateStream?.CopyToAsync(inspectionAgreementTemplateFileStream)!;
            inspectionAgreementTemplateStream.Close();
            inspectionAgreementTemplateFileStream.Close();

            // Replace the placeholder text with the current date   
            var currentDate = DateTime.Today.ToString("M/d/yyyy");

            // Load the DOCX document using Xceed DocX
            using var inspectionAgreementDocument = DocX.Load(InspectionAgreementFilePath);

            inspectionAgreementDocument.ReplaceText("This agreement dated ______.", $"This agreement dated {currentDate}.");
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

            if (!string.IsNullOrEmpty(client.Fee)) inspectionAgreementDocument.ReplaceText("{Inspection Fee}", client.Fee);


            //ReplaceLastDatedInInspectionAgreement(inspectionAgreementDocument, currentDate);

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

            //await AddImageToDocumentAsync(inspectionAgreementDocument, @"https://echifilestorage.blob.core.windows.net/echiphotos/Bathroom 1 b639554a-c715-4084-91d6-8079fb78925a");

            inspectionAgreementDocument.Save();

            // Get the stream from the file path
            await using var fileStream = new FileStream(InspectionAgreementFilePath, FileMode.Open, FileAccess.Read);

            // Pass the stream to your method
            return await azureBlobStorageService.UploadFileToAzureStorage(fileStream, "echidocs",
                $"{client.ClientFirstName} {client.ClientLastName} {client.InspectionAddressLineOne} {Guid.NewGuid()}.docx");
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    private async Task AddImageToDocumentAsync(Document inspectionAgreementDocument, string imageUrl)
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
            var desiredWidth = pageWidth * 0.5f; // Set the image width to half the page width

            // Calculate the scaled height to maintain aspect ratio
            var aspectRatio = image.Width / (float)image.Height;
            var desiredHeight = (int)(desiredWidth / aspectRatio);

            // Resize the image using ImageSharp
            image.Mutate(x => x.Resize((int)desiredWidth, desiredHeight));

            // Convert the resized image back to bytes
            using var resizedImageStream = new MemoryStream();
            image.SaveAsJpeg(resizedImageStream); // Save as JPEG format or other desired format

            // Add the resized image stream to the document
            var newParagraph = inspectionAgreementDocument.InsertParagraph();
            var resizedImage = inspectionAgreementDocument.AddImage(resizedImageStream);
            var picture = resizedImage.CreatePicture();
            newParagraph.AppendPicture(picture);
        }
        catch (Exception ex)
        {
            // Handle or log the exception
            Console.WriteLine($"Error adding image to document: {ex.Message}");
            throw; // Optionally rethrow the exception
        }
    }

    public async Task<bool> CreateRadonAddendum(ClientModel client)
    {
        const string radonAddendumTemplateResourceName =
            "EchiBackendServices.Resources.Documents.Radon Addendum Template.docx";

        Directory.CreateDirectory(DocumentsDirectory); // Create directory if it doesn't exist

        // Save the embedded DOCX resource to the file system

        // Read the embedded resource stream for DOCX
        await using var radonAddendumTemplateStream =
            Assembly.GetExecutingAssembly().GetManifestResourceStream(radonAddendumTemplateResourceName);
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

        return true;
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

internal class DocxTextReplacer
{
    public static void ReplaceTextInDocx(Stream inputStream, string outputFilePath,
        Dictionary<string, string> replacementPatterns)
    {
        using var document = DocX.Load(inputStream);
        if (replacementPatterns.Count <= 0) return;
        ReplaceText(document, replacementPatterns);
        document.SaveAs(outputFilePath);
    }

    private static void ReplaceText(DocX document, Dictionary<string, string> replacementPatterns)
    {
        const string findPattern = "<(.*?)>";
        const RegexOptions regexOptions = RegexOptions.IgnoreCase;
        var newFormatting = new Formatting { Bold = true, FontColor = System.Drawing.Color.Green };

        var replaceTextOptions = new FunctionReplaceTextOptions
        {
            FindPattern = findPattern,
            RegexMatchHandler = (findStr) => ReplaceFunc(findStr, replacementPatterns),
            RegExOptions = regexOptions,
            NewFormatting = newFormatting
        };

        document.ReplaceText(replaceTextOptions);
    }

    private static string ReplaceFunc(string findStr, IReadOnlyDictionary<string, string> replacementPatterns)
    {
        return replacementPatterns.GetValueOrDefault(findStr, findStr);
    }
}