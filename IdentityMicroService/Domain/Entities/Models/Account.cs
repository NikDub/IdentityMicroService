using Microsoft.AspNetCore.Identity;

namespace IdentityMicroService.Domain.Entities.Models
{
    public class Account : IdentityUser
    {
        public int PhotoId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? CreatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }

    }
}
