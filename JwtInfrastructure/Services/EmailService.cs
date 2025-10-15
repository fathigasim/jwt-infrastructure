using System.Net;
using System.Net.Mail;

namespace JwtInfrastructure.Services
{
    public class EmailService : IEmailService
    {

        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var smtpHost = _configuration["EmailSender:Host"]; // e.g., smtp.gmail.com
            var smtpPort = int.Parse(_configuration["EmailSender:Port"]); // e.g., 587
            var fromEmail = _configuration["EmailSender:SenderEmail"];
            var password = _configuration["EmailSender:Password"]; // App password if 2FA

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.Credentials = new NetworkCredential(fromEmail, password);
                client.EnableSsl = true; // 🔑 required for secure connection

                var mailMessage = new MailMessage(fromEmail, to, subject, body)
                {
                    IsBodyHtml = true
                };

                await client.SendMailAsync(mailMessage);
            }
        }

    }
}
