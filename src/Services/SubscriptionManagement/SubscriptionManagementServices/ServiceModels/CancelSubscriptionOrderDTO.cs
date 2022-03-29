namespace SubscriptionManagementServices.ServiceModels;

public class CancelSubscriptionOrderDTO
{
    public CancelSubscriptionOrderDTO()
    {
    }

    public CancelSubscriptionOrderDTO(CancelSubscriptionOrderDTO DTO)
    {
        SubscriptionOrderId = DTO.SubscriptionOrderId;
        MobileNumber = DTO.MobileNumber;
        OperatorId = DTO.OperatorId;
        OperatorName = DTO.OperatorName;
        OrganizationId = DTO.OrganizationId;
        DateOfTermination = DTO.DateOfTermination;
    }

    public Guid SubscriptionOrderId { get; set; }
    public string MobileNumber { get; set; }
    public int OperatorId { get; set; }
    public string OperatorName { get; set; }
    public Guid OrganizationId { get; set; }
    public DateTime DateOfTermination { get; set; }
}