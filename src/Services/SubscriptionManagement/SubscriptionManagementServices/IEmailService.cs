namespace SubscriptionManagementServices
{
    public interface IEmailService
    {
        Task SendAsync(string orderType, Guid subscriptionOrderId, object data, Dictionary<string, string> others = null);
    }
}
