using IdentityMicroService.Application.Dto;
using IdentityMicroService.Domain.Entities.Enums;
using IdentityMicroService.Domain.Entities.Models;

namespace IdentityMicroService.Application.Services.Abstractions
{
    public interface IAccountService
    {
        Task<bool> CreatePatientAsync(RegistrationUserDto model);
        Task<Account> CreateDoctorAsync(DoctorRegistrationDto model);
        Task ChangePhotoAsync(Guid userId, Guid photoId);
        Task<IEnumerable<AccountDto>> GetAccountsByRole(UserRole role);
    }
}
