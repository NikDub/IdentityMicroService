using System.ComponentModel.DataAnnotations;

namespace IdentityMicroService.Domain.Entities.Models.AuthorizationDTO
{
    public class UserModelForAuthorizationDTO
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6)]
        public string password { get; set; }
        public string returnUrl { get; set; }
    }
}
