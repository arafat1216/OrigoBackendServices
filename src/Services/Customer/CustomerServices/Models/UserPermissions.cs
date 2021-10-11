using Common.Seedwork;
using CustomerServices.DomainEvents;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CustomerServices.Models
{
    public class UserPermissions : Entity
    {
        public UserPermissions(User user, Role role, IList<Guid> accessList)
        {
            User = user;
            Role = role;
            AccessList = accessList;
            AddDomainEvent(new UserPermissionAddedDomainEvent(this));
        }

        protected UserPermissions()
        {
        }

        [JsonIgnore]
        public User User { get; set; }

        public Role Role { get; protected set; }

        public IList<Guid> AccessList { get; protected set; }

        public void RemoveAccess(Guid accessId)
        {
            if (AccessList.Remove(accessId))
                AddDomainEvent(new UserAccessRemovedDomainEvent(User.UserId, accessId, AccessList));
        }

        public void AddAccess(Guid accessId)
        {
            AccessList.Add(accessId);
            AddDomainEvent(new UserAccessAddedDomainEvent(User.UserId, accessId, AccessList));
        }

        public void RemoveRole()
        {
            AddDomainEvent(new UserPermissionsRemovedDomainEvent(this));
        }
    }
}