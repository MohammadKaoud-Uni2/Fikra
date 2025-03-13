
using SparkLink.Service.Interface;
using MailKit.Net.Smtp;
using MimeKit;
using SparkLink.Helper;
using Microsoft.AspNetCore.Identity;
using SparkLink.Models.Identity;

namespace SparkLink.Service.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly UserManager<ApplicationUser > _userManager;    
        public EmailService(EmailSettings emaiSettings,UserManager<ApplicationUser> userManager)
        {
            _emailSettings = emaiSettings;
           _userManager = userManager;
        }
        public async Task<string> SendEmail(string Email, string Message, string? reason)
        {
        
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_emailSettings.Host, _emailSettings.portNumber, true);
                await client.AuthenticateAsync(_emailSettings.FromAddress, _emailSettings.Password);
                var bodyBuilder = new BodyBuilder()
                {

                    HtmlBody = $"{Message}",
                    TextBody = "welcome Test"
                };
                var message = new MimeMessage()
                {
                    Body = bodyBuilder.ToMessageBody()

                };
                message.From.Add(new MailboxAddress("SparkLink Company", _emailSettings.FromAddress));
                message.To.Add(new MailboxAddress("testing", Email));
                message.Subject = reason == null ? "new Submitted data" : reason;
                var result = await client.SendAsync(message);
                await client.DisconnectAsync(true);
                if (result != null)
                {
                    return "Success";
                }
                return "FaildSendingMessage";

            }
        }
    }
}


