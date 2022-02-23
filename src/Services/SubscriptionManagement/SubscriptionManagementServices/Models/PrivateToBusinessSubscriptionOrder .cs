namespace SubscriptionManagementServices.Models
{
    public class PrivateToBusinessSubscriptionOrder : SubscriptionOrder
    {
        public PrivateToBusinessSubscriptionOrder() : base()
        {

        }

        public PrivateToBusinessSubscriptionOrder(
            string simCardNumber,
            string simCardActivation,
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
            string operatorName): base(simCardNumber, simCardActivation, subscriptionProductId, oranizationId, operatorAccountId, dataPackageId, orderExecutionDate, mobileNumber, customerReferenceFields, subscriptionAddOnProducts)
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
    }
}
