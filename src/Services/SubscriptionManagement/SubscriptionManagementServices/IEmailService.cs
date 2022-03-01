namespace SubscriptionManagementServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string subject, object data);
    }
}
