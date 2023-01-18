using IdentityMicroService.Application.Dto;
using IdentityMicroService.Domain.Entities.Models;

namespace IdentityMicroService.Application.Services.Abstractions
{
    public interface IAccountService
    {
        Task<bool> CreatePatientAsync(RegistrationUserDto model);
        Task<Account> CreateDoctorAsync(DoctorRegistrationDto model);
        Task ChangePhotoAsync(string userId, string photoId);

    }
}
