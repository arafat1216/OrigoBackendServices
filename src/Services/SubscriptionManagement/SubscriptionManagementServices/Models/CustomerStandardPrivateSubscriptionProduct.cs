using Common.Seedwork;
using SubscriptionManagementServices.DomainEvents;
using System.Text.Json.Serialization;

namespace SubscriptionManagementServices.Models
{
    public class CustomerStandardPrivateSubscriptionProduct : Entity
    {
        public CustomerStandardPrivateSubscriptionProduct()
        {
        }

        public CustomerStandardPrivateSubscriptionProduct(Guid organizationId,string operatorName, string? dataPackage, string subscriptionName, CustomerOperatorSettings customerOperatorSettings, Guid callerId)
        {
            OrganizationId = organizationId;
            OperatorName = operatorName;
            DataPackage = dataPackage;
            SubscriptionName = subscriptionName;
            CustomerOperatorSettings = customerOperatorSettings;
            CreatedBy = callerId;
            AddDomainEvent(new CustomerStandardPrivateSubscriptionProductAddedDomainEvent(this, callerId));

        }
        public Guid OrganizationId { get; set; }
        public string? OperatorName { get; set; }
        public string SubscriptionName { get; set; }
        public string? DataPackage { get; set; }
        public int CustomerOperatorSettingId { get; set; }
        [JsonIgnore]
        public CustomerOperatorSettings CustomerOperatorSettings { get; set; }


    }
}
