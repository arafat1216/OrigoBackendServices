using System;
using AssetServices.DomainEvents;
using Common.Seedwork;

namespace AssetServices.Models
{
    public class DisposeSetting :Entity, IAggregateRoot
    {
        public DisposeSetting(Guid customerId, string payrollContactEmail, Guid callerId)
        {
            CustomerId = customerId;
            CreatedBy = callerId;
            PayrollContactEmail = payrollContactEmail;
        }

        public DisposeSetting()
        {
        }

        /// <summary>
        ///     The external uniquely identifying id across systems.
        /// </summary>
        public Guid ExternalId { get; private set; } = Guid.NewGuid();

        /// <summary>
        ///     The customer for this Dispose Setting.
        /// </summary>
        public Guid CustomerId { get; init; }

        /// <summary>
        ///     Email where notification will ben sent for payroll information.
        /// </summary>
        public string PayrollContactEmail { get; protected set; }

        /// <summary>
        ///     Set Payroll Contact Email for this customer.
        /// </summary>
        /// <param name="payrollContactEmail">The email for updating PayrollContactEmail</param>
        /// <param name="callerId">The userid making this assignment</param>
        public void SetPayrollContactEmail(string payrollContactEmail, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            var previousEmail = PayrollContactEmail;
            PayrollContactEmail = payrollContactEmail;
            AddDomainEvent(new SetPayrollContactEmailDomainEvent(this, callerId, previousEmail));
        }
    }
}
