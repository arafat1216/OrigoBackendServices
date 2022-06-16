using AssetServices.Models;
using Common.Logging;
using System;

namespace AssetServices.DomainEvents
{
    public class SetPayrollContactEmailDomainEvent : BaseEvent
    {
        public SetPayrollContactEmailDomainEvent(DisposeSetting disposeSetting, Guid callerId, string previousEmail) : base(
        callerId)
        {
            DisposeSetting = disposeSetting;
            CallerId = callerId;
            PreviousEmail = previousEmail;
        }

        public DisposeSetting DisposeSetting { get; protected set; }
        public Guid CallerId { get; protected set; }
        public string PreviousEmail { get; protected set; }

        public override string EventMessage()
        {
            return string.IsNullOrEmpty(PreviousEmail)?
                $"Dispose Setting is Set 'Payroll Contact Email' to {DisposeSetting.PayrollContactEmail}."
                : $"Dispose Setting is Changing 'Payroll Contact Email' from {PreviousEmail} to {DisposeSetting.PayrollContactEmail}.";
        }
    }
}
