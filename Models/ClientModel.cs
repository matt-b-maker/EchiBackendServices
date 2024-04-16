using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EchiBackendServices.Models;

public class ClientModel: INotifyPropertyChanged
{
    //Experimental
    [JsonIgnore]public bool IsBusy { get; set; } = false;
    public bool HasSavedInspection { get; set; } = false;
    public string SerializedPageMaster { get; set; } = string.Empty;
    public string ClientDocumentId => $"{ClientFirstName} {ClientLastName} {InspectionAddressLineOne}";
    public string? Guid { get; set; }
    public string? InspectionAgreementDocUrl { get; set; } = string.Empty;
    public string? RadonDocUrl { get; set; } = string.Empty;
    public bool IncludeRadonAddendum { get; set; } = false;

    public bool DoNotIncludeRadonAddendum { get; set; } = false;

    public bool? IsArchived { get; set; } = false;

    public string? UserId { get; set; } = string.Empty;

    public string MainInspectionImageUrl { get; set; } = string.Empty;

    public string MainInspectionImageFileName => $"{ClientFirstName} {ClientLastName} {UserId} Main Inspection Image";

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

    //write clearing method to reset all fields
    public void ClearAllFields()
    {
        IsArchived = false;
        UserId = string.Empty;
        MainInspectionImageUrl = string.Empty;
        IncludeRadonAddendum = false;
        DoNotIncludeRadonAddendum = false;
        ClientEmailAddress = string.Empty;
        ClientFirstName = string.Empty;
        ClientLastName = string.Empty;
        ClientAddressLineOne = string.Empty;
        ClientAddressLineTwo = string.Empty;
        ClientAddressCity = string.Empty;
        ClientAddressState = "NM";
        ClientAddressZipCode = string.Empty;
        ClientPhoneNumber = string.Empty;
        Fee = string.Empty;
        InspectionAddressLineOne = string.Empty;
        InspectionAddressLineTwo = string.Empty;
        InspectionAddressCity = string.Empty;
        InspectionAddressState = "NM";
        InspectionAddressZipCode = string.Empty;
        AgencyName = string.Empty;
        AgentName = string.Empty;
        AgentPhoneNumber = string.Empty;
        AgentEmail = string.Empty;
        InspectionDateString = DateTime.Today.ToString("M/d/yyyy h:mm:ss tt");
        //InspectionTime = null;
        //InspectionTimeString = string.Empty;
        PresentAtInspection = string.Empty;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}