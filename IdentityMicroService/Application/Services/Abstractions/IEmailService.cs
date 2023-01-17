using IdentityMicroService.Application.Dto;
using IdentityMicroService.Domain.Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMicroService.Application.Services.Abstractions;

public interface IEmailService
{
    Task<bool> SendEmailConfirmAsync(RegistrationUserDto user, IUrlHelper url);

    Task<bool> ConfirmEmailAsync(string email, string token);
    Task<bool> CheckExistsEmail(string email);
    Task<bool> SendEmailConfirmForDoctorAsync(Account model, IUrlHelper url);

}