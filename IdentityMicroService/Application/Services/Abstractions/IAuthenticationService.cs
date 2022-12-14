using IdentityMicroService.Domain.Entities.Enums;
using IdentityMicroService.Domain.Entities.Models;
using IdentityMicroService.Domain.Entities.Models.AuthorizationDTO;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMicroService.Application.Services.Abstractions
{
    public interface IAuthenticationService
    {
        Task<Account> ReturnUserIfValidAsync(UserModelForAuthorizationDTO userForAuthentication);
        Task<(string accessToken, string refreshToken)> GetTokensAsync(UserModelForAuthorizationDTO userForAuthentication);
        Task<Account> CreateUserAsync(RegistrationUserDTO model);
        Task<bool> CreatePatientAsync(RegistrationUserDTO model);
        Task AddUserRoleAsync(Account user, UserRole role);
        Task SignOutAsync();
        Task<Account> GetUserById(string userId);
        Task<bool> SendEmailConfirmAsync(RegistrationUserDTO user, IUrlHelper url);

        Task<bool> ConfirmEmailAsync(string email, string token);
        Task<bool> CheckExistsEmail(string email);
    }
}
