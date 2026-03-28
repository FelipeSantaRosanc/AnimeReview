using AnimeReview.Services.Interfaces;
using MimeKit;

namespace AnimeReview.Services;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public SmtpEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            _configuration["Email:FromName"] ?? "AnimeReview",
            _configuration["Email:FromEmail"] ?? "noreply@animereview.com"));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
        message.Body = bodyBuilder.ToMessageBody();

        // Usar MailKit SmtpClient explicitamente
        using var client = new MailKit.Net.Smtp.SmtpClient();

        var smtpServer = _configuration["Email:SmtpServer"] ?? "smtp.gmail.com";
        var smtpPort = _configuration.GetValue<int>("Email:SmtpPort", 587);

        await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);

        var username = _configuration["Email:SmtpUsername"];
        var password = _configuration["Email:SmtpPassword"];

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            await client.AuthenticateAsync(username, password);
        }

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendConfirmationEmailAsync(string to, string userName, string confirmationLink)
    {
        var subject = "Confirm your AnimeReview account";
        var body = $@"
            <h1>Welcome to AnimeReview, {userName}!</h1>
            <p>Please confirm your email by clicking the link below:</p>
            <a href='{confirmationLink}'>Confirm Email</a>";

        await SendEmailAsync(to, subject, body);
    }

    public async Task SendPasswordResetEmailAsync(string to, string userName, string resetLink)
    {
        var subject = "Reset your AnimeReview password";
        var body = $@"
            <h1>Password Reset Request</h1>
            <p>Hello {userName},</p>
            <p>Click the link below to reset your password:</p>
            <a href='{resetLink}'>Reset Password</a>";

        await SendEmailAsync(to, subject, body);
    }
}