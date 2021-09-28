using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class UserPermissionsRemovedDomainEvent : BaseEvent
    {
        public UserPermissionsRemovedDomainEvent(UserPermissions userPermissions) : base(userPermissions.User.UserId)
        {
            UserPermissions = userPermissions;
        }

        public UserPermissions UserPermissions { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"User permissions removed.";
        }
    }
}
