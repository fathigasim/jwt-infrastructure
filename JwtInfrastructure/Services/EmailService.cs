using System.Net;
using System.Net.Mail;

namespace JwtInfrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtp;
        public EmailService(IConfiguration config)
        {
            _smtp = new SmtpClient(config["Smtp:Host"], int.Parse(config["Smtp:Port"]))
            {
                Credentials = new NetworkCredential(config["Smtp:User"], config["Smtp:Pass"]),
                EnableSsl = true
            };
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var mail = new MailMessage("no-reply@yourapp.com", to, subject, body);
            mail.IsBodyHtml = false;
            await _smtp.SendMailAsync(mail);
        }
    }

}
