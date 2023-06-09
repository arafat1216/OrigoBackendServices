﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Enums;
using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class TransferToPrivateSubscriptionOrder : Entity, ISubscriptionOrder
    {
        public TransferToPrivateSubscriptionOrder()
        {

        }

        public PrivateSubscription UserInfo { get; set; }
        public string MobileNumber { get; set; }
        public string OperatorName { get; set; }
        public string NewSubscription { get; set; }
        public DateTime OrderExecutionDate { get; set; }
        public Guid OrganizationId { get; set; }

        #region ISubscriptionOrder implementation

        public Guid SubscriptionOrderId { get; set; } = Guid.NewGuid();
        [NotMapped] public SubscriptionOrderTypes OrderType => SubscriptionOrderTypes.TransferToPrivate;
        [NotMapped] public string PhoneNumber => MobileNumber;

        [NotMapped]
        public string NewSubscriptionOrderOwnerName => $"{UserInfo?.FirstName} {UserInfo?.LastName}";
        [NotMapped] public DateTime ExecutionDate => OrderExecutionDate;
        public string? SalesforceTicketId { get; set; }
        #endregion
    }
}