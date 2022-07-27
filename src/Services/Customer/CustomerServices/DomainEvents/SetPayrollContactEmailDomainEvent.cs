using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class SetPayrollContactEmailDomainEvent : BaseEvent
    {
        public SetPayrollContactEmailDomainEvent(Organization organization, string previousEmail) : base(
        organization.OrganizationId)
        {
            Organization = organization;
            PreviousEmail = previousEmail;
        }

        public Organization Organization { get; protected set; }
        public string PreviousEmail { get; protected set; }

        public override string EventMessage()
        {
            return string.IsNullOrEmpty(PreviousEmail) ?
                $"'Payroll Contact Email' is set to {Organization.PayrollContactEmail}."
                : $"'Payroll Contact Email' is changed from {PreviousEmail} to {Organization.PayrollContactEmail}.";
        }
    }
}
