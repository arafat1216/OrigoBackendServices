namespace SubscriptionManagementServices.ServiceModels
{
    public class ActivateSimOrderDTOResponse
    {
        public ActivateSimOrderDTOResponse()
        {
        }

        public ActivateSimOrderDTOResponse(ActivateSimOrderDTOResponse DTO)
        {
            MobileNumber = DTO.MobileNumber;
            OperatorName = DTO.OperatorName;
            SimCardNumber = DTO.SimCardNumber;
            SimCardType = DTO.SimCardType;
            CreatedBy = DTO.CreatedBy;
        }

        public string MobileNumber { get; set; }
        public string OperatorName { get; set; }
        public string SimCardNumber { get; set; }
        public string SimCardType { get; set; }
        public Guid CreatedBy { get; set; } 
    }
}
