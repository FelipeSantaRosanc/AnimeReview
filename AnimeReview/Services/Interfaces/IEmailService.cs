namespace AnimeReview.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlBody);
        Task SendConfirmationEmailAsync(string to, string userName, string confirmationLink);
        Task SendPasswordResetEmailAsync(string to, string userName, string resetLink);

    }
}
