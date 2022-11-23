using System.ComponentModel.DataAnnotations;

namespace IdentityMicroService.Domain.Entities.Models.AuthorizationDTO
{
    public class UserModelForAuthorizationDTO
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6)]
        public string Password { get; set; }
        [Required]
        public string ReturnUrl { get; set; }
    }
}
