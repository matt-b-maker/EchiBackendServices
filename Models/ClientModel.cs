namespace EchiBackendServices.Models;

public class ClientModel
{
    public bool IncludeRadonAddendum { get; set; } = false;

    public bool DoNotIncludeRadonAddendum { get; set; } = false;

    public bool? IsArchived { get; set; } = false;

    public string? UserId { get; set; } = string.Empty;

    public string MainInspectionImageUrl { get; set; } = string.Empty;

    public string MainInspectionImageFileName { get; set; }
    public string? ClientEmailAddress { get; set; }

    public string? ClientFirstName { get; set; }

    public string? ClientLastName { get; set; }

    public string? ClientAddressLineOne { get; set; }

    public string? ClientAddressLineTwo { get; set; }

    public string? ClientAddressCity { get; set; }

    public string? ClientAddressState { get; set; } = "NM";

    public string? ClientAddressZipCode { get; set; }

    public string? ClientPhoneNumber { get; set; } = string.Empty;

    public string? Fee { get; set; } = string.Empty;
    public string? RadonFee { get; set; } = string.Empty;

    public string InspectionAddressLineOne { get; set; } = string.Empty;
    public string InspectionAddressLineTwo { get; set; } = string.Empty;
    public string InspectionAddressCity { get; set; } = string.Empty;
    public string InspectionAddressState { get; set; } = "NM";
    public string InspectionAddressZipCode { get; set; } = string.Empty;

    public string AgencyName { get; set; } = string.Empty;

    public string AgentName { get; set; } = string.Empty;
    public string AgentPhoneNumber { get; set; } = string.Empty;
    public string AgentEmail { get; set; } = string.Empty;

    public string? InspectionDateString { get; set; }

    public string PresentAtInspection { get; set; } = string.Empty;

    public string InspectionFullAddress =>
        $"{InspectionAddressLineOne} {InspectionAddressCity} {InspectionAddressState} {InspectionAddressZipCode}";
}