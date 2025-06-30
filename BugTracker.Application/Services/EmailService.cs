using BugTracker.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace BugTracker.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpSection = _configuration.GetSection("SmtpSettings");

            var smtpClient = new SmtpClient(smtpSection["Host"])
            {
                Port = int.Parse(smtpSection["Port"]),
                Credentials = new NetworkCredential(smtpSection["Username"], smtpSection["Password"]),
                EnableSsl = bool.Parse(smtpSection["EnableSsl"])
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSection["From"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
