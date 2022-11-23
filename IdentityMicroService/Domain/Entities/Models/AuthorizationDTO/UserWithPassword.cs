using System.ComponentModel.DataAnnotations;

namespace IdentityMicroService.Domain.Entities.Models.AuthorizationDTO
{
    public class UserWithPassword
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
