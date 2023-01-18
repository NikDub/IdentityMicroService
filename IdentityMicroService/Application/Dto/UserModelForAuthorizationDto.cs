using System.ComponentModel.DataAnnotations;

namespace IdentityMicroService.Application.Dto;

public class UserModelForAuthorizationDto
{
    [Required][EmailAddress] public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6)]
    public string Password { get; set; }

    public string ReturnUrl { get; set; }
}