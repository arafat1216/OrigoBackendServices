﻿using Common.Seedwork;
using SubscriptionManagementServices.DomainEvents;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubscriptionManagementServices.Models
{
    public class ActivateSimOrder : Entity, ISubscriptionOrder
    {
        public ActivateSimOrder()
        {
        }

        public ActivateSimOrder(string mobileNumber, string @operator, string simNumber, string simType, Guid organizationId, Guid callerId)
        {
            MobileNumber = mobileNumber;
            OperatorName = @operator;
            SimCardNumber = simNumber;
            SimCardType = simType;
            OrganizationId = organizationId;
            SubscriptionOrderId = Guid.NewGuid();
            CreatedBy = callerId;
            AddDomainEvent(new ActivateSimOrderCreatedDomainEvent(this, callerId));
        }

        public string MobileNumber { get; set; }
        public string OperatorName { get; set; }
        public string SimCardNumber { get; set; } 
        public string SimCardType { get; set; }
        public Guid OrganizationId { get; set; }

        #region ISubscriptionOrder Implementation

        [NotMapped] public string OrderType => $"ActivateSimCard - {SimCardType}";

        [NotMapped] public string PhoneNumber => MobileNumber;

        [NotMapped] public string NewSubscriptionOrderOwnerName => MobileNumber != null ? MobileNumber : "Order reference not specified";

        [NotMapped] public DateTime TransferDate => DateTime.UtcNow;

        public Guid SubscriptionOrderId { get; set; }
        public string? SalesforceTicketId { get; set; }
        #endregion
    }
}
