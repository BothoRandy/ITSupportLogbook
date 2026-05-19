using MailKit.Net.Smtp;
using MimeKit;

namespace ITSupportLogbook.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendPasswordResetAsync(string toEmail, string resetLink)
        {
            var from = _config["Email__From"] ?? throw new Exception("Email__From not configured");
            var host = _config["Email__Host"] ?? throw new Exception("Email__Host not configured");
            var port = _config["Email__Port"] ?? throw new Exception("Email__Port not configured");
            var username = _config["Email__Username"] ?? throw new Exception("Email__Username not configured");
            var password = _config["Email__Password"] ?? throw new Exception("Email__Password not configured");

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(from));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "IT Support Logbook - Password Reset";

            message.Body = new TextPart("html")
            {
                Text = $"""
            <p>You requested a password reset for your IT Support Logbook account.</p>
            <p><a href="{resetLink}">Click here to reset your password</a></p>
            <p>If you did not request this, ignore this email.</p>
        """
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(host, int.Parse(port), MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(username, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}