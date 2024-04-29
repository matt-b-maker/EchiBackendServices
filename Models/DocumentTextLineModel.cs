namespace EchiBackendServices.Models;
using Color = System.Drawing.Color;

public class DocumentTextLineModel(string sectionName, string lineText, List<string> imageUrls, Color? color = null)
{
    public string SectionName { get; set; } = sectionName;
    public string LineText { get; set; } = lineText;
    public Color? Color { get; set; } = color ?? System.Drawing.Color.Black;
}