
namespace SubscriptionManagementServices.Exceptions
{
    public class CustomerSettingsException : SubscriptionManagementException
    {
        public CustomerSettingsException()
        {
        }

        public CustomerSettingsException(string message) : base(message)
        {
        }

        public CustomerSettingsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
