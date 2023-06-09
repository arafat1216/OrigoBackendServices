﻿using Common.Seedwork;
using System.ComponentModel.DataAnnotations;

namespace SubscriptionManagementServices.Models
{
    public class CustomerOperatorAccount : Entity
    {
        public CustomerOperatorAccount()
        {

        }

        /// <summary>
        /// Used for testing to be able to set identity id.
        /// </summary>
        public CustomerOperatorAccount(int id, Guid organizationId, string connectedOrganizationNumber,string accountNumber, string accountName, int operatorId,
            Guid callerId) : this(organizationId, connectedOrganizationNumber, accountNumber, accountName, operatorId, callerId)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Id = id;
        }

        public CustomerOperatorAccount(Guid organizationId, string? connectedOrganizationNumber, string accountNumber, string accountName, int operatorId, Guid callerId)
        {
            OrganizationId = organizationId;
            ConnectedOrganizationNumber = connectedOrganizationNumber ?? string.Empty;
            AccountNumber = accountNumber;
            AccountName = accountName.Length <= 40 ? accountName : accountName[..40];
            OperatorId = operatorId;
            CreatedBy = callerId;
            UpdatedBy = callerId;
        }

        public Guid OrganizationId { get; set; }
        [MaxLength(50)]
        public string AccountNumber { get; set; }
        [MaxLength(40)]
        public string AccountName { get; set; }
        public virtual Operator Operator { get; set; }
        public int OperatorId { get; set; }
        public string? ConnectedOrganizationNumber { get; set; }
        public virtual CustomerOperatorSettings CustomerOperatorSetting { get; set; }
    }
}
