
namespace Bible.API.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string email, string subject, string message);
}
