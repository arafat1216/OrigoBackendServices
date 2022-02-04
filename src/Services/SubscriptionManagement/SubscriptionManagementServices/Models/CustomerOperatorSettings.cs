using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class CustomerOperatorSettings : Entity
    {
        public CustomerOperatorSettings()
        {
        }
        public int CustomerOperatorSettingsId { get; protected set; }
        public Operator Operator { get; protected set; }
        public IReadOnlyCollection<SubscriptionProduct>? SubscriptionProducts { get; protected set; }
        public virtual CustomerSettings CustomerSetting { get; set; }
    }
}