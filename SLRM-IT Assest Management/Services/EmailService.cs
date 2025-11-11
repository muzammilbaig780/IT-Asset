using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace SLRM_IT_Assest_Management.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendPasswordResetEmail(string toEmail, string resetLink)
        {
            var apiKey = _configuration["SendGrid:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("SendGrid API key is missing in appsettings.json");

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("noreply@yourdomain.com", "SLRM IT Asset Management");
            var subject = "Password Reset Request";
            var to = new EmailAddress(toEmail);
            var htmlContent = $"Click the link to reset your password: <a href='{resetLink}'>Reset Password</a>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to send email via SendGrid");
            }
        }
    }
}
