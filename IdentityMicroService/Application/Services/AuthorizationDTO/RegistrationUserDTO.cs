using System.ComponentModel.DataAnnotations;

namespace IdentityMicroService.Domain.Entities.Models.AuthorizationDTO
{
    public class RegistrationUserDTO : UserModelForAuthorizationDTO
    {
        [Required(ErrorMessage = "Confirmation Password is required.")]
        [Compare("Password", ErrorMessage = "Password and Confirmation Password must match.")]
        public string ConfirmPassword { get; set; }
    }
}
