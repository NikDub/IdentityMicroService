using IdentityMicroService.Application.Dto;
using IdentityMicroService.Application.Services.Abstractions;
using IdentityMicroService.Domain.Entities.Enums;
using IdentityMicroService.Domain.Entities.Models;
using IdentityMicroService.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace IdentityMicroService.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<Account> _userManager;
        private readonly SignInManager<Account> _signInManager;
        private readonly ApplicationDbContext _dBContext;
        private readonly IAuthenticationService _authenticationService;

        public AccountService(
            UserManager<Account> userManager,
        SignInManager<Account> signInManager,
        ApplicationDbContext dBContext,
        IAuthenticationService authenticationService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dBContext = dBContext;
            _authenticationService = authenticationService;
        }
        public async Task<Account> CreateDoctorAsync(DoctorRegistrationDto model)
        {
            var transaction = await _dBContext.Database.BeginTransactionAsync();
            Account user;
            try
            {
                user = await CreateUserDoctorAsync(model);

                await _authenticationService.AddUserRoleAsync(user, UserRole.Doctor);

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                return null;
            }

            return user;
        }
        public async Task<bool> CreatePatientAsync(RegistrationUserDto model)
        {
            var transaction = await _dBContext.Database.BeginTransactionAsync();

            try
            {
                var user = await _authenticationService.CreateUserAsync(model);

                await _authenticationService.AddUserRoleAsync(user, UserRole.Patient);

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"{nameof(CreatePatientAsync)} - Can't create patient or add role.");
                return false;
            }

            return true;
        }
        public async Task<Account> CreateUserDoctorAsync(DoctorRegistrationDto model)
        {
            var password = Guid.NewGuid().ToString("N").ToLower()
                .Replace("1", "").Replace("o", "").Replace("0", "")
                .Substring(0, 10);
            var result =
                await _userManager.CreateAsync(
                    new Account { Email = model.Email, UserName = model.Email, PhotoId = model.PhotoId }, password);
            if (!result.Succeeded) return null;

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return null;


            var res = await _signInManager.PasswordSignInAsync(user, password, false, false);
            user.PasswordHash = password;
            if (res.Succeeded) return user;

            return null;
        }
        public async Task ChangePhotoAsync(string userId, string photoId)
        {
            var user = await _authenticationService.GetUserById(userId);
            user.PhotoId = photoId;
            await _userManager.UpdateAsync(user);
        }
    }
}
