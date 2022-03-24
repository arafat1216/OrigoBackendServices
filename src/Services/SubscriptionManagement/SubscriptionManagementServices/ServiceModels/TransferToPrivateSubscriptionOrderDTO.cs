﻿namespace SubscriptionManagementServices.ServiceModels
{
    public class TransferToPrivateSubscriptionOrderDTO
    {
        public TransferToPrivateSubscriptionOrderDTO()
        {
        }

        public TransferToPrivateSubscriptionOrderDTO(TransferToPrivateSubscriptionOrderDTO DTO)
        {
            PrivateSubscription = DTO.PrivateSubscription;
            MobileNumber = DTO.MobileNumber;
            OperatorName = DTO.OperatorName;
            NewSubscription = DTO.NewSubscription;
            OrderExecutionDate = DTO.OrderExecutionDate;
            CallerId = DTO.CallerId;
        }

        public PrivateSubscriptionDTO PrivateSubscription { get; set; }
        public string MobileNumber { get; set; }
        public string OperatorName { get; set; }
        public string NewSubscription { get; set; }
        public DateTime OrderExecutionDate { get; set; }
        public Guid CallerId { get; set; }
    }
}
