using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class SetLastDayOfReportingSalaryDeductionDomainEvent : BaseEvent
    {
        public SetLastDayOfReportingSalaryDeductionDomainEvent(Organization organization, int previousDate) : base(
        organization.OrganizationId)
        {
            Organization = organization;
            PreviousDate = previousDate;
        }

        public Organization Organization { get; protected set; }
        public int PreviousDate { get; protected set; }

        public override string EventMessage()
        {
            return $"'Last day of Reporting Salary Deduction' is changed from {PreviousDate} to {Organization.LastDayForReportingSalaryDeduction}.";
        }
    }
}
