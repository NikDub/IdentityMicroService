using IdentityMicroService.Application.Services.AuthorizationDTO;
using IdentityMicroService.Domain.Entities.Enums;
using IdentityMicroService.Domain.Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMicroService.Application.Services.Abstractions;

public interface IAuthenticationService
{
    Task<Account> ReturnUserIfValidAsync(UserModelForAuthorizationDto userForAuthentication);
    Task<(string accessToken, string refreshToken)> GetTokensAsync(UserModelForAuthorizationDto userForAuthentication);
    Task<bool> CreatePatientAsync(RegistrationUserDto model);
    Task AddUserRoleAsync(Account user, UserRole role);
    Task SignOutAsync();
    Task<Account> GetUserById(string userId);
    Task<bool> SendEmailConfirmAsync(RegistrationUserDto user, IUrlHelper url);

    Task<bool> ConfirmEmailAsync(string email, string token);
    Task<bool> CheckExistsEmail(string email);
    Task<Account> CreateDoctorAsync(DoctorRegistrationDto model);
    Task<bool> SendEmailConfirmForDoctorAsync(Account model, IUrlHelper url);
    Task ChangePhotoAsync(string userId, string photoId);
}