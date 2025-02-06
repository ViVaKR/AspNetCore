using ViVaBM.API.Interfaces;

namespace ViVaBM.API.Services;

public class EmailService(IConfiguration config) : IEmailService
{
    private readonly IConfiguration _config = config;
    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        // var mailServer = _config["MailServer"];
        await Task.CompletedTask;
    }
}
