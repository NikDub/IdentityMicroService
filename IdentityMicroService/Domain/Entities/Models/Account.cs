using Microsoft.AspNetCore.Identity;

namespace IdentityMicroService.Domain.Entities.Models;

public class Account : IdentityUser
{
    public string PhotoId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }
}