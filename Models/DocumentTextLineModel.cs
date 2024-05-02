namespace EchiBackendServices.Models;
using Color = System.Drawing.Color;

public class DocumentTextLineModel(string sectionName, string lineText, List<string> imageUrls, string? color = null)
{
    public string SectionName { get; set; } = sectionName;
    public string LineText { get; set; } = lineText;
    public string? Color { get; set; } = color ?? "Black";
}