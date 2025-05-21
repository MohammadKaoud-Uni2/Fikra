using SparkLink.Service.Interface;
using MailKit.Net.Smtp;
using MimeKit;
using SparkLink.Helper;
using Microsoft.AspNetCore.Identity;
using SparkLink.Models.Identity;
using MimeKit.Utils;

namespace SparkLink.Service.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmailService(EmailSettings emaiSettings, UserManager<ApplicationUser> userManager)
        {
            _emailSettings = emaiSettings;
            _userManager = userManager;
        }

        public async Task<string> SendEmail(string Email, string Message, string? reason)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            using (var client = new SmtpClient())
            {
                try
                {
                    // SMTP2Go recommended connection settings
                    // Port numbers: 2525 (recommended), 8025, 587 or 80
                    // Using 2525 as it's SMTP2Go's primary port
                    await client.ConnectAsync("mail.smtp2go.com", 2525, MailKit.Security.SecureSocketOptions.Auto);

                    // Hardcoded credentials as requested
                    await client.AuthenticateAsync("pixaf64894@neuraxo.com", "P4OT6epVM8wSixcj");

                    var bodyBuilder = new BodyBuilder()
                    {
                        HtmlBody = Message,
                        TextBody = !string.IsNullOrWhiteSpace(Message) ?
                                   System.Text.RegularExpressions.Regex.Replace(Message, "<[^>]*>", "") :
                                   "Hello"
                    };

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("SparkLink Company", "fikra@mailna.co"));
                    message.To.Add(new MailboxAddress(user?.UserName ?? "VipUser", Email));
                    message.Subject = reason ?? "New Submitted Data";
                    message.Body = bodyBuilder.ToMessageBody();
                    message.MessageId = MimeUtils.GenerateMessageId("sparklink");

                    // Recommended headers
                    message.Headers.Add("X-Mailer", "SparkLink");
                    message.Headers.Add("MIME-Version", "1.0");
                    message.Headers.Add("X-Auto-Response-Suppress", "All");
                    message.Headers.Add("Precedence", "bulk");

                    var response = await client.SendAsync(message);
                    await client.DisconnectAsync(true);

                    return "Success";
                }
                catch (SmtpCommandException ex)
                {
                    // SMTP-specific error
                    return $"SMTP Error ({ex.StatusCode}): {ex.Message}";
                }
                catch (Exception ex)
                {
                    // General error
                    return $"Failed: {ex.Message}";
                }
            }
        }
    }
}