using System.ComponentModel.DataAnnotations;

namespace SubscriptionManagementServices.ServiceModels;

public record NewCancelSubscriptionOrder
{
    [Phone]
    [MaxLength(15)]
    public string MobileNumber { get; set; }
    public int OperatorId { get; set; }
    public DateTime DateOfTermination { get; set; }
    public Guid CallerId { get; set; }
}