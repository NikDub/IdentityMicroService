﻿using IdentityMicroService.Domain.Entities.Models;
using IdentityMicroService.Domain.Entities.Models.AuthorizationDTO;

namespace IdentityMicroService.Application.Services.Abstractions
{
    public interface IAuthenticationService
    {
        Task<Account> ReturnUserIfValidAsync(UserModelForAuthorizationDTO userForAuthentication);
        Task<(string accessToken, string refreshToken)> GetTokensAsync(UserModelForAuthorizationDTO userForAuthentication);
        Task<Account> CreateUserAsync(RegistrationUserDTO model);
        Task SignOutAsync();
    }
}