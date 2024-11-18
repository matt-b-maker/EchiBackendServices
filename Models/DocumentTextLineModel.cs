using System.Text.Json.Serialization;

namespace EchiBackendServices.Models;

public class DocumentTextLineModel(string sectionName, string lineText, string? color = null)
{
    [JsonPropertyName("sectionName")]
    public string SectionName { get; set; } = sectionName;

    [JsonPropertyName("lineText")]
    public string LineText { get; set; } = lineText;

    [JsonPropertyName("color")]
    public string? Color { get; set; } = color ?? "Black";
}