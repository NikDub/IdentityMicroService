using System.Net;
using System.Net.Mail;
using IdentityMicroService.Application.Services.Abstractions;

namespace IdentityMicroService.Application.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool SendEmail(string email, string message)
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