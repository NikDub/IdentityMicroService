using IdentityMicroService.Application.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Net;
using System.Net.Mail;

namespace IdentityMicroService.Application.Services
{
    public class EmailService : IEmailService
    {
        public IConfiguration _configuration { get; }

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool SendEmail(string email, string message)
        {
            MailMessage emailMessage = new MailMessage();

            emailMessage.From = new MailAddress(_configuration.GetValue<string>("SMTP:From"), _configuration.GetValue<string>("SMTP:FromName"));
            emailMessage.To.Add(new MailAddress(email));
            emailMessage.Subject = "Confirm your email";
            emailMessage.Body = message;

            var client = new SmtpClient(_configuration.GetValue<string>("SMTP:Host"), _configuration.GetValue<int>("SMTP:Port"))
            {
                Credentials = new NetworkCredential(_configuration.GetValue<string>("SMTP:EmailCred"), _configuration.GetValue<string>("SMTP:PasswordCred")),
                EnableSsl = true,
            };

            try
            {
                client.Send(emailMessage);
                return true;
            }
            catch (Exception){}
            return false;
        }
    }
}
