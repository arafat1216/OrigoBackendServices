using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class PrivateToBusinessSubscriptionOrder : Entity, ISubscriptionOrder
    {
        private List<SubscriptionAddOnProduct> _subscriptionAddOnProducts;

        public PrivateToBusinessSubscriptionOrder()
        {

        }

        public PrivateToBusinessSubscriptionOrder(
            string simCardNumber,
            string simCardAction,
            int subscriptionProductId,
            Guid oranizationId,
            int operatorAccountId,
            int dataPackageId,
            DateTime orderExecutionDate,
            string mobileNumber,
            string customerReferenceFields,
            List<SubscriptionAddOnProduct> subscriptionAddOnProducts,
            string firstName,
            string lastName,
            string address,
            string postalPlace,
            string postalCode,
            string country,
            string email,
            DateTime dob,
            string operatorName)
        {
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            PostalCode = postalCode;
            PostalPlace = postalPlace;
            Country = country;
            Email = email;
            BirthDate = dob;
            OperatorName = operatorName;

            SimCardNumber = simCardNumber;
            SIMCardAction = simCardAction;
            SubscriptionProductId = subscriptionProductId;
            OrganizationId = oranizationId;
            OperatorAccountId = operatorAccountId;
            DataPackageId = dataPackageId;
            OrderExecutionDate = orderExecutionDate;
            MobileNumber = mobileNumber;
            CustomerReferenceFields = customerReferenceFields;
            _subscriptionAddOnProducts = subscriptionAddOnProducts;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string PostalPlace { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string OperatorName { get; set; }
        public CustomerSubscriptionProduct CustomerSubscriptionProduct { get; set; }

        public IReadOnlyCollection<SubscriptionAddOnProduct> SubscriptionAddOnProducts => _subscriptionAddOnProducts.AsReadOnly();

        public string SimCardNumber { get; set; }
        public string SIMCardAction { get; set; }
        public int SubscriptionProductId { get; set; }
        public Guid OrganizationId { get; set; }
        public CustomerOperatorAccount OperatorAccount { get; set; }
        public int OperatorAccountId { get; set; }
        public DataPackage? DataPackage { get; set; }
        public int DataPackageId { get; set; }
        public DateTime OrderExecutionDate { get; set; }
        public string MobileNumber { get; set; }
        public string CustomerReferenceFields { get; set; }

        public void SetSubscriptionAddOnProduct(List<SubscriptionAddOnProduct> subscriptionAddOnProducts)
        {
            _subscriptionAddOnProducts.AddRange(subscriptionAddOnProducts);
        }
    }
}
