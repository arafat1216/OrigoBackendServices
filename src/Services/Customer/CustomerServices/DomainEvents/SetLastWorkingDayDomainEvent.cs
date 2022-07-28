using Common.Logging;
using CustomerServices.Models;
using System;

namespace CustomerServices.DomainEvents
{
    class SetLastWorkingDayDomainEvent : BaseEvent
    {
        public SetLastWorkingDayDomainEvent(User user, DateTime? lastWorkingDay, Guid callerId) : base(user.UserId)
        {
            User = user;
            LastWorkingDay = lastWorkingDay;
            CallerId = CallerId;
        }

        public User User { get; protected set; }
        public DateTime? LastWorkingDay { get; protected set; }
        public User CallerId { get; protected set; }

        public override string EventMessage()
        {
            string logMessage;
            if (User.LastWorkingDay != null && LastWorkingDay != null)
                logMessage = $"'LastWorkingDay' is changed from {User.LastWorkingDay.Value} to {LastWorkingDay.Value} for User : {User.Id} by {CallerId}";
            else if (User.LastWorkingDay != null && LastWorkingDay == null)
                logMessage = $"'LastWorkingDay' :{User.LastWorkingDay.Value} is removed for User : {User.Id} by {CallerId}";
            else if (LastWorkingDay != null)
                logMessage = $"'LastWorkingDay' is set {LastWorkingDay.Value} for User : {User.Id} by {CallerId}";
            else
                logMessage = $"'LastWorkingDay' is set {LastWorkingDay} for User : {User.Id} by {CallerId}";

            return logMessage;
        }
    }
}
