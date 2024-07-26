﻿namespace Buddham.SharedLib.Contracts;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}
