using System.Net;
using System.Net.Mail;
using System.Text;
using ViVaBM.API.Interfaces;

namespace ViVaBM.API.Services;

public class EmailService(IConfiguration config) : IEmailService
{
    private readonly IConfiguration _config = config;
    public async Task<bool> SendEmailAsync(string toEmail, string subject, string message)
    {
        var mailServer = _config["EmailSettings:MailServer"] ?? "smtp.gmail.com";
        var fromEmail = _config["EmailSettings:FromEmail"] ?? "viva.viva.bm@gmail.com";
        var password = _config["EmailSettings:FromEmailPassword"] ?? "-";
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
