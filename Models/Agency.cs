using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EchiBackendServices.Models;

public class Agency
{
    [Key] public int Id { get; set; }
    [Column(TypeName = "nvarchar(100)")] public string? UserId { get; set; } = string.Empty;
    [Column(TypeName = "nvarchar(100)")] public string? AgencyName { get; set; } = string.Empty;
}