
namespace SubscriptionManagementServices.ServiceModels
{
    public class CancelSubscriptionOrderDTOResponse
    {
        public CancelSubscriptionOrderDTOResponse()
        {
        }

        public CancelSubscriptionOrderDTOResponse(CancelSubscriptionOrderDTOResponse DTO)
        {
            SubscriptionOrderId = DTO.SubscriptionOrderId;
            MobileNumber = DTO.MobileNumber;
            OperatorName = DTO.OperatorName;
            DateOfTermination = DTO.DateOfTermination;
            CreatedBy = DTO.CreatedBy;
        }

        public Guid SubscriptionOrderId { get; set; }
        public string MobileNumber { get; set; }
        
        public string OperatorName { get; set; }
        public DateTime DateOfTermination { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
