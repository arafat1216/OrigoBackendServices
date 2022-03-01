namespace SubscriptionManagementServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(object data);
    }
}
