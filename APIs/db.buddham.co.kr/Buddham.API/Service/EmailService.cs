using System.Net;
using System.Net.Mail;
using System.Text;
using Buddham.SharedLib.Contracts;

namespace Buddham.API.Service;

public class EmailService(IConfiguration config) : IEmailService
{
    private readonly IConfiguration _config = config;

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        string? MailServer = _config["EmailSettings:MailServer"]!;
        string? FromEmail = _config["EmailSettings:FromEmail"]!;
        string? Password = _config["EmailSettings:Password"]!;
        int Port = Convert.ToInt32(_config["EmailSettings:MailPort"]!);

        var client = new SmtpClient(MailServer, Port)
        {
            Credentials = new NetworkCredential(FromEmail, Password),
            EnableSsl = true,
            UseDefaultCredentials = false
        };

        MailMessage mailMessage = new() { From = new MailAddress(FromEmail) };
        mailMessage.To.Add(toEmail);
        mailMessage.Subject = subject;
        mailMessage.IsBodyHtml = true;
        StringBuilder mailBody = new();
        mailBody.AppendFormat($"<h3>{subject}</h3>");
        mailBody.AppendFormat($"<br />");
        mailBody.AppendFormat($"<p>{body}</p>");

        mailMessage.Body = mailBody.ToString();

        await client.SendMailAsync(mailMessage);
        return;
    }

    public async Task ConfirmSendEmailAsync(string toEmail, string subject, string body)
    {
        string? MailServer = _config["EmailSettings:MailServer"]!;
        string? FromEmail = _config["EmailSettings:FromEmail"]!;
        string? Password = _config["EmailSettings:Password"]!;
        int Port = Convert.ToInt32(_config["EmailSettings:MailPort"]!);

        var client = new SmtpClient(MailServer, Port)
        {
            Credentials = new NetworkCredential(FromEmail, Password),
            EnableSsl = true,
            UseDefaultCredentials = false
        };

        MailMessage mailMessage = new() { From = new MailAddress(FromEmail) };
        mailMessage.To.Add(toEmail);
        mailMessage.Subject = subject;
        mailMessage.IsBodyHtml = true;
        StringBuilder mailBody = new();
        mailBody.AppendFormat($"<h3>{subject}</h3>");
        mailBody.AppendFormat($"<br />");
        mailBody.AppendFormat($"<p>{body}</p>");

        mailMessage.Body = mailBody.ToString();

        await client.SendMailAsync(mailMessage);
        return;
    }
}
