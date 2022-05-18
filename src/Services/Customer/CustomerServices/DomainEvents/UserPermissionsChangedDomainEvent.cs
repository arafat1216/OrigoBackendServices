using Common.Logging;
using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.DomainEvents
{
    public class UserPermissionsChangedDomainEvent : BaseEvent
    {
        public UserPermissionsChangedDomainEvent(UserPermissions userPermissions, Role previousRole) : base(userPermissions.User.UserId)
        {
            UserPermissions = userPermissions;
            PreviousRole = previousRole;
        }

        public UserPermissions UserPermissions { get; set; }
        public Role PreviousRole { get; set; }

        public override string EventMessage()
        {
            return PreviousRole != null ? 
                $"User permissions updated from {PreviousRole.Name} to {UserPermissions.Role.Name}" :
                $"User permission updated to {UserPermissions.Role.Name}";
        }
    }
}
