﻿using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class SubscriptionOrder : Entity
    {
        public SubscriptionOrder()
        {

        }
        public SubscriptionOrder(Guid customerId, int subscriptionProductId, int operatorAccountId, int dataPackageId, Guid callerId, string simCardNumber)
        {
            CustomerId = customerId;
            SubscriptionProductId = subscriptionProductId;
            OperatorAccountId = operatorAccountId;
            DataPackageId = dataPackageId;
            CreatedBy = callerId;
            UpdatedBy = callerId;
            SimCardNumber = simCardNumber;
        }

        public virtual SubscriptionProduct SubscriptionProduct { get; set; }
        public virtual IReadOnlyCollection<SubscriptionAddOnProduct> SubscriptionAddOnProducts { get; set; }
        public string SimCardNumber { get; set; }
        public int SubscriptionProductId { get; set; }
        public Guid CustomerId { get; set; }
        public virtual CustomerOperatorAccount OperatorAccount { get; set; }
        public int OperatorAccountId { get; set; }
        public virtual DataPackage? DataPackage { get; set; }
        public int DataPackageId { get; set; }
        public DateTime OrderExecutionDate { get; set; }
    }
}
