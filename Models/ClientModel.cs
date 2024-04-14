using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EchiBackendServices.Models;

public class ClientModel
{
    [Key] public int Id { get; set; }

    [Column(TypeName = "nvarchar(max)")] public string? SerializedPageMaster { get; set; } = string.Empty;

    public bool IncludeRadonAddendum { get; set; } = false;

    public bool DoNotIncludeRadonAddendum { get; set; } = false;

    public bool? IsArchived { get; set; } = false;

    [Column(TypeName = "nvarchar(100)")] public string? UserId { get; set; } = string.Empty;

    [Column(TypeName = "nvarchar(100)")] public string MainInspectionImageUrl { get; set; } = string.Empty;

    [Column(TypeName = "nvarchar(100)")] public string? MainInspectionImageFileName { get; set; }

    [Column(TypeName = "nvarchar(100)")] public string? Guid { get; set; }

    [Column(TypeName = "nvarchar(100)")] public string? ClientFullName => $"{ClientFirstName} {ClientLastName}";

    [Column(TypeName = "nvarchar(100)")] public string? ClientEmailAddress { get; set; }

    [Column(TypeName = "nvarchar(100)")] public string? ClientFirstName { get; set; }

    [Column(TypeName = "nvarchar(100)")] public string? ClientLastName { get; set; }

    [Column(TypeName = "nvarchar(100)")] public string? ClientAddressLineOne { get; set; }

    [Column(TypeName = "nvarchar(100)")] public string? ClientAddressLineTwo { get; set; }

    [Column(TypeName = "nvarchar(100)")] public string? ClientAddressCity { get; set; }

    [Column(TypeName = "nvarchar(100)")] public string? ClientAddressState { get; set; } = "NM";

    [Column(TypeName = "nvarchar(100)")] public string? ClientAddressZipCode { get; set; }

    [Column(TypeName = "nvarchar(100)")] public string? ClientPhoneNumber { get; set; } = string.Empty;

    [Column(TypeName = "nvarchar(50)")] public string? Fee { get; set; } = string.Empty;

    [Column(TypeName = "nvarchar(50)")] public string? RadonFee { get; set; } = string.Empty;

    [Column(TypeName = "nvarchar(100)")] public string InspectionAddressLineOne { get; set; } = string.Empty;

    [Column(TypeName = "nvarchar(100)")] public string InspectionAddressLineTwo { get; set; } = string.Empty;

    [Column(TypeName = "nvarchar(100)")] public string InspectionAddressCity { get; set; } = string.Empty;

    [Column(TypeName = "nvarchar(100)")] public string InspectionAddressState { get; set; } = "NM";
    [Column(TypeName = "nvarchar(100)")] public string InspectionAddressZipCode { get; set; } = string.Empty;
    [Column(TypeName = "nvarchar(100)")] public string AgencyName { get; set; } = string.Empty;
    [Column(TypeName = "nvarchar(100)")] public string AgentName { get; set; } = string.Empty;
    [Column(TypeName = "nvarchar(100)")] public string AgentPhoneNumber { get; set; } = string.Empty;
    [Column(TypeName = "nvarchar(100)")] public string AgentEmail { get; set; } = string.Empty;

    [Column(TypeName = "nvarchar(100)")] public string? InspectionDateString { get; set; }

    [Column(TypeName = "nvarchar(100)")] public string PresentAtInspection { get; set; } = string.Empty;

    [Column(TypeName = "nvarchar(100)")]
    public string InspectionFullAddress =>
        $"{InspectionAddressLineOne} {InspectionAddressCity} {InspectionAddressState} {InspectionAddressZipCode}";
}