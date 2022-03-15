using Common.Logging;
using CustomerServices.Models;


namespace CustomerServices.DomainEvents
{
    class UserUpdateEmployeeIdDomainEvent : BaseEvent
    {
        public UserUpdateEmployeeIdDomainEvent(User user, string oldEmplyeeId) : base(user.UserId)
        {
            User = user;
            OldEmployeeId = oldEmplyeeId;
        }

        public User User { get; protected set; }

        public string OldEmployeeId { get; protected set; }

        public override string EventMessage()
        {
            return $"User employeeId changed from {OldEmployeeId} to {User.EmployeeId}.";
        }
    }
}
