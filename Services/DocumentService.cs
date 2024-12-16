using EchiBackendServices.Models;
using System.Reflection;
using System.Runtime.CompilerServices;
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
using SixLabors.ImageSharp.Formats.Jpeg;

namespace EchiBackendServices.Services;

public class DocumentService(AzureBlobStorageService azureBlobStorageService)
{
    private static readonly object _docLock = new();

    public string DocumentsDirectory { get; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Documents");

    public string InspectionAgreementFilePath { get; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Inspection Agreement.docx");

    public string RadonAddendumFilePath { get; set; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Radon Addendum.docx");

    public string InspectionReportFilePath { get; set; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Inspection Report.docx");


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

            // Load the DOCX document using Xceed DocX
            using var inspectionAgreementDocument = DocX.Load(InspectionAgreementFilePath);

            inspectionAgreementDocument.ReplaceText(new StringReplaceTextOptions
            {
                SearchValue = "This agreement dated ______",
                NewValue = $"This agreement dated {currentDate}"
            });
            inspectionAgreementDocument.ReplaceText(new StringReplaceTextOptions
            {
                SearchValue = "{today’s date}",
                NewValue = $" {currentDate}"
            });
            inspectionAgreementDocument.ReplaceText(new StringReplaceTextOptions
            {
                SearchValue = "Client: __________________",
                NewValue = $"Client: {client.ClientFirstName} {client.ClientLastName}"
            });
            if (!string.IsNullOrEmpty(client.ClientFirstName) || !string.IsNullOrEmpty(client.ClientLastName))
                inspectionAgreementDocument.ReplaceText(new StringReplaceTextOptions
                {
                    SearchValue = "{name}",
                    NewValue = $"{client.ClientFirstName} {client.ClientLastName}"
                });
            if (!string.IsNullOrEmpty(client.ClientAddressLineOne))
                inspectionAgreementDocument.ReplaceText(new StringReplaceTextOptions
                {
                    SearchValue = "(Address 1}",
                    NewValue = client.ClientAddressLineOne
                });
            if (!string.IsNullOrEmpty(client.ClientAddressLineTwo))
                inspectionAgreementDocument.ReplaceText(new StringReplaceTextOptions
                {
                    SearchValue = "{address 2}",
                    NewValue = client.ClientAddressLineTwo
                });
            if (!string.IsNullOrEmpty(client.ClientAddressCity) ||
                !string.IsNullOrEmpty(client.ClientAddressState) ||
                !string.IsNullOrEmpty(client.ClientAddressZipCode))
            {
                var replaceOptions = new StringReplaceTextOptions
                {
                    SearchValue = "{City, state zip}",
                    NewValue = $"{client.ClientAddressCity}, {client.ClientAddressState} {client.ClientAddressZipCode}"
                };
                inspectionAgreementDocument.ReplaceText(replaceOptions);
            }

            if (!string.IsNullOrEmpty(client.ClientPhoneNumber))
            {
                var replaceOptions = new StringReplaceTextOptions
                {
                    SearchValue = "{phone}",
                    NewValue = client.ClientPhoneNumber
                };
                inspectionAgreementDocument.ReplaceText(replaceOptions);
            }

            if (!string.IsNullOrEmpty(client.ClientEmailAddress))
            {
                var replaceOptions = new StringReplaceTextOptions
                {
                    SearchValue = "{Email}",
                    NewValue = client.ClientEmailAddress
                };
                inspectionAgreementDocument.ReplaceText(replaceOptions);
            }

            if (!string.IsNullOrEmpty(client.InspectionAddressLineOne))
            {
                var replaceOptions = new StringReplaceTextOptions
                {
                    SearchValue = "{Inspection address 1}",
                    NewValue = client.InspectionAddressLineOne
                };
                inspectionAgreementDocument.ReplaceText(replaceOptions);
            }

            if (!string.IsNullOrEmpty(client.InspectionAddressLineTwo))
            {
                var replaceOptions = new StringReplaceTextOptions
                {
                    SearchValue = "{Inspection address 2}",
                    NewValue = client.InspectionAddressLineTwo
                };
                inspectionAgreementDocument.ReplaceText(replaceOptions);
            }

            if (!string.IsNullOrEmpty(client.InspectionAddressCity) ||
                !string.IsNullOrEmpty(client.InspectionAddressState) ||
                !string.IsNullOrEmpty(client.InspectionAddressZipCode))
            {
                var replaceOptions = new StringReplaceTextOptions
                {
                    SearchValue = "{inspection city, state zip}",
                    NewValue =
                        $"{client.InspectionAddressCity}, {client.InspectionAddressState} {client.InspectionAddressZipCode}"
                };
                inspectionAgreementDocument.ReplaceText(replaceOptions);
            }

            if (!string.IsNullOrEmpty(client.FeeWithTaxes))
            {
                var replaceOptions = new StringReplaceTextOptions
                {
                    SearchValue = "{Inspection Fee}",
                    NewValue = $"${client.FeeWithTaxes.Trim('$')}"
                };
                inspectionAgreementDocument.ReplaceText(replaceOptions);
            }

            //if (inspectionAgreementDocument.Text.Contains("{inspection city, state zip}"))
            //{
            //    Console.WriteLine("");
            //}

            var stringsToRemove = GetListOfStringsToRemove(client);
            var strToRemoves = stringsToRemove ?? [.. stringsToRemove];
            if (strToRemoves.Count > 0)
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

        radonDocument.ReplaceText(new StringReplaceTextOptions
        {
            SearchValue = "dated _____________",
            NewValue = $"dated {currentDate}"
        });

        radonDocument.ReplaceText(new StringReplaceTextOptions
        {
            SearchValue = "$________",
            NewValue = $"{client.RadonFeeWithTaxes}"
        });

        radonDocument.ReplaceText(new StringReplaceTextOptions
        {
            SearchValue = "Client:",
            NewValue = $"Client: {client.ClientFirstName} {client.ClientLastName}"
        });

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

    public async Task<string> CreateInspectionReport(ClientModel client,
        List<DocumentTextLineModel> inspectionReportLines, List<DocumentImageModel>? images)
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
        inspectionReportDocument.ReplaceText(new StringReplaceTextOptions
        {
            SearchValue = "{Name}",
            NewValue = $"{client.ClientFirstName} {client.ClientLastName}"
        });
        inspectionReportDocument.ReplaceText(new StringReplaceTextOptions
        {
            SearchValue = "{ClientAddressLine1}",
            NewValue = $"{client.ClientAddressLineOne}"
        });
        inspectionReportDocument.ReplaceText(new StringReplaceTextOptions
        {
            SearchValue = "{ClientAddressLine2}",
            NewValue = !string.IsNullOrEmpty(client.ClientAddressLineTwo)
                ? $"{client.ClientAddressLineTwo}"
                : string.Empty
        });
        inspectionReportDocument.ReplaceText(new StringReplaceTextOptions
        {
            SearchValue = "{Client City State ZIP}",
            NewValue = $"{client.ClientAddressCity}, {client.ClientAddressState} {client.ClientAddressZipCode}"
        });
        inspectionReportDocument.ReplaceText(new StringReplaceTextOptions
        {
            SearchValue = "{Client Phone}",
            NewValue = $"{client.ClientPhoneNumber}"
        });
        inspectionReportDocument.ReplaceText(new StringReplaceTextOptions
        {
            SearchValue = "{Client Email}",
            NewValue = $"{client.ClientEmailAddress}"
        });
        // replace {Today’s Date} with today's date
        inspectionReportDocument.ReplaceText(new StringReplaceTextOptions
        {
            SearchValue = "{Today’s Date}",
            NewValue = $"{DateTime.Today:MM/dd/yyyy}"
        });

        //fill in inspection address info
        inspectionReportDocument.ReplaceText(new StringReplaceTextOptions
        {
            SearchValue = "{InspectionAddressLine1}",
            NewValue = $"{client.InspectionAddressLineOne}"
        });
        inspectionReportDocument.ReplaceText(new StringReplaceTextOptions
        {
            SearchValue = "{InspectionAddressLine2}",
            NewValue = !string.IsNullOrEmpty(client.InspectionAddressLineTwo)
                ? $"{client.InspectionAddressLineTwo}"
                : string.Empty
        });
        inspectionReportDocument.ReplaceText(new StringReplaceTextOptions
        {
            SearchValue = "{Inspection City State ZIP}",
            NewValue =
                $"{client.InspectionAddressCity}, {client.InspectionAddressState} {client.InspectionAddressZipCode}"
        });
        inspectionReportDocument.ReplaceText(new StringReplaceTextOptions
        {
            SearchValue = "{Selling Agent}",
            NewValue = $"{client.AgentName}"
        });

        //replace text {inspection image} with image from url
        var inspectionImageParagraph =
            inspectionReportDocument.Paragraphs.FirstOrDefault(p => p.Text.Contains("{inspection image}"));

        if (inspectionImageParagraph != null && !string.IsNullOrEmpty(client.MainInspectionImageUrl))
        {
            var imageUrl = client.MainInspectionImageUrl;
            await AddImageToDocumentAsync(inspectionReportDocument, imageUrl, inspectionImageParagraph, 0.6f, true);
            inspectionReportDocument.ReplaceText(new StringReplaceTextOptions()
            {
                SearchValue = "{inspection image}",
                NewValue = string.Empty
            });
        }

        //Grounds Section
        await AddLinesToReport(inspectionReportLines, images, InspectionSections.GroundsSection,
            inspectionReportDocument, "{grounds}");

        //Exterior Walls Section
        await AddLinesToReport(inspectionReportLines, images, InspectionSections.ExteriorWallsSection,
            inspectionReportDocument, "{exterior walls}");

        //Roofing Section
        await AddLinesToReport(inspectionReportLines, images, InspectionSections.RoofingSection,
            inspectionReportDocument, "{roofing}");

        //Windows and Doors Section
        await AddLinesToReport(inspectionReportLines, images, InspectionSections.WindowsAndDoorsSection,
            inspectionReportDocument, "{windows and doors}");

        //Attic Space Section
        await AddLinesToReport(inspectionReportLines, images, InspectionSections.AtticSpaceSection,
            inspectionReportDocument, "{attic space}");

        //Foundation Section
        await AddLinesToReport(inspectionReportLines, images, InspectionSections.FoundationSection,
            inspectionReportDocument, "{foundation}");

        //Interior W/C/F Section
        await AddLinesToReport(inspectionReportLines, images, InspectionSections.InteriorWcfSection,
            inspectionReportDocument, "{interior w/c/f}");

        //Heating Section
        await AddLinesToReport(inspectionReportLines, images, InspectionSections.HeatingSection,
            inspectionReportDocument, "{heating}");

        //Electric Section
        await AddLinesToReport(inspectionReportLines, images, InspectionSections.ElectricSection,
            inspectionReportDocument, "{electric}");

        //Plumbing Section
        await AddLinesToReport(inspectionReportLines, images, InspectionSections.PlumbingSection,
            inspectionReportDocument, "{plumbing}");

        //Kitchen Section
        await AddLinesToReport(inspectionReportLines, images, InspectionSections.KitchenSection,
            inspectionReportDocument, "{kitchen}");

        //Bathrooms Section
        await AddLinesToReport(inspectionReportLines, images, InspectionSections.BathroomsSection,
            inspectionReportDocument, "{bathrooms}");

        //Laundry Section
        await AddLinesToReport(inspectionReportLines, images, InspectionSections.LaundrySection,
            inspectionReportDocument, "{laundry}");

        //Garage Section
        await AddLinesToReport(inspectionReportLines, images, InspectionSections.GarageSection,
            inspectionReportDocument, "{garage}");

        //Outdoor Living Space Section
        await AddLinesToReport(inspectionReportLines, images, InspectionSections.OutdoorLivingSpaceSection,
            inspectionReportDocument, "{outdoor living space}");

        //Additional Comments Section
        await AddLinesToReport(inspectionReportLines, images, InspectionSections.AdditionalCommentsSection,
            inspectionReportDocument, "{additional comments}");

        // Save the document
        inspectionReportDocument.Save();

        // Get the stream from the file path
        await using var fileStream = new FileStream(InspectionReportFilePath, FileMode.Open, FileAccess.Read);

        // Upload file to Azure Blob Storage and return the URL
        return await azureBlobStorageService.UploadFileToAzureStorage(fileStream, "echidocs",
            $"{client.ClientFirstName} {client.ClientLastName} {client.InspectionAddressLineOne} Inspection Report {Guid.NewGuid()}.docx");
    }

    public async Task AddLinesToReport(List<DocumentTextLineModel> inspectionReportLines,
        List<DocumentImageModel>? images, string section, Document inspectionReportDocument, string replaceString)
    {
        var sectionLines = inspectionReportLines.Where(l => l.SectionName == section).ToList();

        if (sectionLines.Count == 0) return;

        var targetParagraph = inspectionReportDocument.Paragraphs.FirstOrDefault(p => p.Text.Contains(replaceString));

        if (targetParagraph is null) return;

        foreach (var line in sectionLines)
        {
            if (line.LineText.Contains("IMAGETOINSERT|"))
            {
                var lineTextSplit = line.LineText.Split('|');
                if (lineTextSplit.Length < 2) continue;
                var imageUrl = lineTextSplit.Last();                
                await AddImageToDocumentAsync(inspectionReportDocument, imageUrl, targetParagraph, 0.25f);
                continue;
            }

            //translate color property to System.Drawing.Color
            var color = Color.FromName(line.Color ?? "Black");
            targetParagraph?.InsertText(line.LineText + "\n", false, new Formatting() {FontColor = color});
        }

        if (images?.Count > 0 && images.Any(x => x.SectionName == section))
        {
            var sectionedImages = images.Where(i => i.SectionName == section).ToList();
            foreach (var image in sectionedImages)
            {
                await AddImageToDocumentAsync(inspectionReportDocument, image.ImageUrl, targetParagraph, 0.25f);
            }
        }

        inspectionReportDocument.ReplaceText(new StringReplaceTextOptions
        {
            SearchValue = replaceString,
            NewValue = string.Empty
        });
    }

    private static async Task AddImageToDocumentAsync(Document document, string imageUrl, Paragraph targetParagraph,
        float imageWidth = 0.5f, bool centerImage = false)
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
            var pageWidth = document.PageWidth;
            var desiredWidth = pageWidth * imageWidth; // Set the image width to half the page width

            // Calculate the scaled height to maintain aspect ratio
            var aspectRatio = image.Width / (float) image.Height;
            var desiredHeight = (int) (desiredWidth / aspectRatio);

            // Resize the image using ImageSharp
            image.Mutate(x =>
            {
                x.Resize((int) desiredWidth, desiredHeight);
                x.AutoOrient();
            });

            // Convert the resized image back to bytes
            // Generate a temporary file path with .jpg extension
            var tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".jpg");
            await image.SaveAsync(tempFilePath);

            // Add the resized image stream to the document
            //var newParagraph = document.InsertParagraph(paragraphIndex, "", false);
            var resizedImage = document.AddImage(tempFilePath);
            var picture = resizedImage.CreatePicture();
            targetParagraph.AppendPicture(picture);

            // Optionally center the image
            if (centerImage)
            {
                targetParagraph.Alignment = Alignment.center;
            }
        }
        catch (Exception ex)
        {
            // Handle or log the exception
            Console.WriteLine($"Error adding image to document: {ex.Message}");
            throw; // Optionally rethrow the exception
        }
    }

    private static void ReplaceLastDatedInInspectionAgreement(Container doc, string currentDate)
    {
        // Iterate through paragraphs in reverse order
        for (var i = doc.Paragraphs.Count - 1; i >= 0; i--)
        {
            var paragraph = doc.Paragraphs[i];
            var lines = paragraph.Text.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

            for (var j = lines.Length - 1; j >= 0; j--)
            {
                var line = lines[j];
                var lastIndex = line.LastIndexOf("Dated:", StringComparison.Ordinal);
                if (lastIndex == -1) continue;
                // Replace only the last occurrence of "Dated:" in the line
                var updatedLine = line[..lastIndex] + $"Dated: {currentDate}" + line[(lastIndex + "Dated:".Length)..];
                paragraph.ReplaceText(new StringReplaceTextOptions
                {
                    SearchValue = line,
                    NewValue = updatedLine
                });
                return; // Exit the loop after replacing the text in the last occurrence
            }
        }
    }

    private static List<string> GetListOfStringsToRemove(ClientModel client)
    {
        var listOfStrings = new List<string>();
        if (string.IsNullOrEmpty(client.ClientFirstName) && string.IsNullOrEmpty(client.ClientLastName))
            listOfStrings.Add("{name}");
        if (string.IsNullOrEmpty(client.ClientAddressLineOne)) listOfStrings.Add("(Address 1}");
        if (string.IsNullOrEmpty(client.ClientAddressLineTwo)) listOfStrings.Add("{address 2}");
        if (string.IsNullOrEmpty(client.ClientAddressCity) || string.IsNullOrEmpty(client.ClientAddressState) ||
            string.IsNullOrEmpty(client.ClientAddressZipCode)) listOfStrings.Add("{City, state zip}");
        if (string.IsNullOrEmpty(client.ClientPhoneNumber)) listOfStrings.Add("{phone}");
        if (string.IsNullOrEmpty(client.ClientEmailAddress)) listOfStrings.Add("{Email}");
        if (string.IsNullOrEmpty(client.InspectionAddressLineOne)) listOfStrings.Add("{Inspection address 1}");
        if (string.IsNullOrEmpty(client.InspectionAddressLineTwo)) listOfStrings.Add("{Inspection address 2}");
        if (string.IsNullOrEmpty(client.InspectionAddressCity) || string.IsNullOrEmpty(client.InspectionAddressState) ||
            string.IsNullOrEmpty(client.InspectionAddressZipCode)) listOfStrings.Add("{inspection city, state zip}");
        if (string.IsNullOrEmpty(client.FeeWithTaxes)) listOfStrings.Add("{Inspection Fee}");

        return listOfStrings;
    }
}