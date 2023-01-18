using System.Net;
using System.Net.Mail;
using IdentityMicroService.Application.Dto;
using IdentityMicroService.Application.Services.Abstractions;
using IdentityMicroService.Domain.Entities.Models;
using IdentityMicroService.Presentation.Extensions;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMicroService.Application.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<Account> _userManager;

    public EmailService(IConfiguration configuration, UserManager<Account> userManager)
    {
        _configuration = configuration;
        this._userManager = userManager;
    }

    public async Task<bool> SendEmailConfirmAsync(RegistrationUserDto model, IUrlHelper url)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null) return false;

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        if (token.IsNullOrEmpty()) return false;

        var confirmationLink = url.Action(NameOfMethods.AuthActionConfirmEmail, NameOfMethods.AuthControllerName,
            new { token, email = model.Email }, PathConfiguration.Https);
        if (confirmationLink == null) return false;

        var emailResponse = SendEmail(model.Email, confirmationLink);
        return emailResponse;
    }
    public async Task<bool> SendEmailConfirmForDoctorAsync(Account model, IUrlHelper url)
    {
        if (model == null)
            return false;

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return false;

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        if (token.IsNullOrEmpty()) return false;

        var confirmationLink = url.Action(NameOfMethods.AuthActionConfirmEmail, NameOfMethods.AuthControllerName,
            new { token, email = model.Email }, PathConfiguration.Https);
        if (confirmationLink == null) return false;

        confirmationLink = $"Username: {model.UserName}\n" +
                                       $"Password: {model.PasswordHash}\n" +
                                       $"{confirmationLink}";

        var emailResponse = SendEmail(model.Email, confirmationLink);
        return emailResponse;
    }
    public async Task<bool> ConfirmEmailAsync(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return false;

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded) return true;

        return false;
    }
    public async Task<bool> CheckExistsEmail(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user != null;
    }
    private bool SendEmail(string email, string message)
    {
        var emailMessage = new MailMessage();

        emailMessage.From = new MailAddress(_configuration.GetValue<string>("SMTP:SendFromEmail"),
            _configuration.GetValue<string>("SMTP:SendFromEmailName"));
        emailMessage.To.Add(new MailAddress(email));
        emailMessage.Subject = _configuration.GetValue<string>("SMTP:EmailTitle");
        emailMessage.Body = message;

        var client = new SmtpClient(_configuration.GetValue<string>("SMTP:Host"),
            _configuration.GetValue<int>("SMTP:Port"))
        {
            Credentials = new NetworkCredential(_configuration.GetValue<string>("SMTP:EntryCredEmail"),
                _configuration.GetValue<string>("SMTP:EntryCredPassword")),
            EnableSsl = true
        };

        try
        {
            client.Send(emailMessage);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}