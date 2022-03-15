using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class UserPermissionAddedDomainEvent : BaseEvent
    {
        public UserPermissionAddedDomainEvent(UserPermissions userPermissions) : base(userPermissions.User.UserId)
        {
            UserPermissions = userPermissions;
        }

        public UserPermissions UserPermissions { get; protected set; }

        public override string EventMessage()
        {
            return $"User permissions added.";
        }
    }
}
