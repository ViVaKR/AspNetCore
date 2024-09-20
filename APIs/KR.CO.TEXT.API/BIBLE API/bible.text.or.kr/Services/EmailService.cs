using System.Net;
using System.Net.Mail;
using System.Text;
using Bible.API.Interfaces;

namespace Bible.API.Services;

public class EmailService(IConfiguration config) : IEmailService
{
    private readonly IConfiguration _config = config;
    public async Task<bool> SendEmailAsync(string toEmail, string subject, string message)
    {
        var mailServer = _config["EmailSettings:MailServer"];
        var fromEmail = _config["EmailSettings:FromEmail"];
        var password = _config["EmailSettings:Password"];
        var mailPort = Convert.ToInt32(_config["EmailSettings:MailPort"] ?? "587");

        var smtpClient = new SmtpClient(mailServer, mailPort)
        {
            Credentials = new NetworkCredential(fromEmail, password),
            EnableSsl = true,
            UseDefaultCredentials = false
        };

        MailMessage mailMessage = new() { From = new MailAddress(fromEmail) };
        mailMessage.To.Add(toEmail);
        mailMessage.Subject = subject;
        mailMessage.IsBodyHtml = true;

        StringBuilder mailBody = new();
        mailBody.AppendFormat($"<h1>{subject}</h1>");
        mailBody.AppendFormat("<br />");
        mailBody.AppendFormat($"<p>{message}</p>");
        mailMessage.Body = mailBody.ToString();

        await smtpClient.SendMailAsync(mailMessage);
        return true;
    }
}
