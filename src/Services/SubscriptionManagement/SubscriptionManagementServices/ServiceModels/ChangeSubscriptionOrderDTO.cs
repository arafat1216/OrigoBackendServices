
namespace SubscriptionManagementServices.ServiceModels
{
    public record ChangeSubscriptionOrderDTO
    {
        public ChangeSubscriptionOrderDTO()
        {
        }

        public ChangeSubscriptionOrderDTO( ChangeSubscriptionOrderDTO DTO)
        {
            MobileNumber = DTO.MobileNumber;
            OperatorName = DTO.OperatorName;
            ProductName = DTO.ProductName;
            PackageName = DTO.PackageName;
            SubscriptionOwner = DTO.SubscriptionOwner;
            OrganizationId = DTO.OrganizationId;
        }

            public string MobileNumber { get; set; }
            public string OperatorName { get; set; }
            public string ProductName { get; set; }
            public string? PackageName { get; set; }
            public string? SubscriptionOwner { get; set; }
            public Guid OrganizationId { get; set; }

        
    }
}
