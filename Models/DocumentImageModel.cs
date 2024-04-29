namespace EchiBackendServices.Models;

public class DocumentImageModel(string imageUrl, string sectionName)
{
    public string ImageUrl { get; set; } = imageUrl;
    public string SectionName { get; set; } = sectionName;
}