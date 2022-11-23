using System.ComponentModel.DataAnnotations;

namespace IdentityMicroService.Domain.Entities.Models.AuthorizationDTO
{
    public class UserModelForAuthorizationDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ReturnUrl { get; set; }
    }
}
