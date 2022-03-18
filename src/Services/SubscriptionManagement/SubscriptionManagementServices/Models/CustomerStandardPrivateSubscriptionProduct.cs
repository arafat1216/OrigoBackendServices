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

        public CustomerStandardPrivateSubscriptionProduct(Guid organizationId, string? dataPackage, string subscriptionName, CustomerSubscriptionProduct? customerSubscriptionProduct, CustomerOperatorSettings customerOperatorSettings, Guid callerId)
        {
            OrganizationId = organizationId;
            OperatorName = CustomerOperatorSettings.Operator.OperatorName;
            DataPackage = dataPackage;
            SubscriptionName = subscriptionName;
            CustomerSubscriptionProduct = customerSubscriptionProduct;
            CustomerOperatorSettings = customerOperatorSettings;
            CreatedBy = callerId;
            AddDomainEvent(new CustomerStandardPrivateSubscriptionProductAddedDomainEvent(this, callerId));

        }
        public Guid OrganizationId { get; set; }
        public string? OperatorName { get; set; }
        public string SubscriptionName { get; set; }
        public string? DataPackage { get; set; }
        [JsonIgnore]
        public CustomerSubscriptionProduct? CustomerSubscriptionProduct { get; set; }
        [JsonIgnore]
        public CustomerOperatorSettings CustomerOperatorSettings { get; set; }


    }
}
