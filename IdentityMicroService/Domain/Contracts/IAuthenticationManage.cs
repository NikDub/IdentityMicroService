using IdentityMicroService.Domain.Entities.Models;
using IdentityMicroService.Domain.Entities.Models.AuthorizationDTO;

namespace IdentityMicroService.Domain.Contracts
{
    public interface IAuthenticationManage
    {
        Task<Accounts> ReturnUserIfValid(UserModelForAuthorizationDTO userForAuthentication);
        Task<(string accessToken, string refreshToken)> GetTokensAsync(UserModelForAuthorizationDTO userForAuthentication);
        Task<Accounts> CreateUser(RegistrationUserDTO model);
        Task SignOut();
    }
}
