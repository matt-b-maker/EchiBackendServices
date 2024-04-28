namespace EchiBackendServices.Models;

public class InspectionReportModel
{
    public List<DocumentTextLineModel>? InspectionReportLines { get; set; }
    public ClientModel? Client { get; set; }
}