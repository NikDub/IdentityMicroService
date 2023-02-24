using IdentityMicroService.Application.Dto;
using IdentityMicroService.Domain.Entities.Enums;
using IdentityMicroService.Domain.Entities.Models;

namespace IdentityMicroService.Application.Services.Abstractions;

public interface IAuthenticationService
{
    Task<Account> IsUserExistsAsync(UserModelForAuthorizationDto userForAuthentication);
    Task<bool> UserSingInAsync(Account user, UserModelForAuthorizationDto userForAuthentication);
    Task<(string accessToken, string refreshToken)> GetTokensAsync(UserModelForAuthorizationDto userForAuthentication);
    Task AddUserRoleAsync(Account user, UserRole role);
    Task SignOutAsync();
    Task<Account> GetUserById(Guid userId);
    Task<Account> CreateUserAsync(RegistrationUserDto model);
}