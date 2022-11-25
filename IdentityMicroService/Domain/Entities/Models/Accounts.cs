using Microsoft.AspNetCore.Identity;

namespace IdentityMicroService.Domain.Entities.Models
{
    public class Accounts : IdentityUser
    {
        public int PhotoId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? CreatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }

    }
}
