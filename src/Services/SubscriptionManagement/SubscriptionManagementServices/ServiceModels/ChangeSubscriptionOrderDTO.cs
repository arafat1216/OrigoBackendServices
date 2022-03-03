
namespace SubscriptionManagementServices.ServiceModels
{
    public record ChangeSubscriptionOrderDTO
    {
            public string MobileNumber { get; set; }
            public string OperatorName { get; set; }
            public string ProductName { get; set; }
            public string? PackageName { get; set; }
            public string? SubscriptionOwner { get; set; }
            public Guid OrganizationId { get; set; }

        
    }
}
