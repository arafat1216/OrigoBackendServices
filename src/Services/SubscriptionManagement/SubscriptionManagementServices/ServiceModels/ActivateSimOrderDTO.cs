
namespace SubscriptionManagementServices.ServiceModels
{
    public class ActivateSimOrderDTO
    {
        public ActivateSimOrderDTO()
        {
        }

        public ActivateSimOrderDTO(ActivateSimOrderDTO DTO)
        {
            MobileNumber = DTO.MobileNumber;
            OperatorName = DTO.OperatorName;
            OperatorId = DTO.OperatorId;
            SimCardNumber = DTO.SimCardNumber;
            SimCardType = DTO.SimCardType;
        }

        public string MobileNumber { get; set; }
        public int OperatorId { get; set; }
        public string OperatorName { get; set; }
        public string SimCardNumber { get; set; }
        public string SimCardType { get; set; }
    }
}
