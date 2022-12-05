namespace IdentityMicroService.Application.Services.Abstractions
{
    public interface IEmailService
    {
        bool SendEmail(string email, string message);
    }
}
