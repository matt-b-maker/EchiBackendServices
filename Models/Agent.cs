using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EchiBackendServices.Models
{
    public class Agent
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(100)")] public string? UserId { get; set; }
        [Column(TypeName = "nvarchar(100)")] public string AgencyName { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(100)")] public string AgentName { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(100)")] public string AgentPhoneNumber { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(100)")] public string AgentEmail { get; set; } = string.Empty;
    }
}