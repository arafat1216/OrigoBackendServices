﻿using Common.Seedwork;
using CustomerServices.DomainEvents;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CustomerServices.Models
{
    public class UserPermissions : Entity
    {
        public UserPermissions(User user, Role role, IList<Guid> accessList, Guid callerId)
        {
            User = user;
            Role = role;
            AccessList = accessList;
            CreatedBy = callerId;
            AddDomainEvent(new UserPermissionAddedDomainEvent(this));
        }

        protected UserPermissions()
        {
        }

        [JsonIgnore]
        public User User { get; set; }

        public Role Role { get; protected set; }

        public IList<Guid> AccessList { get; protected set; }

        public void RemoveAccess(Guid accessId,Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.Now;
            if (AccessList.Remove(accessId))
            {
                AddDomainEvent(new UserAccessRemovedDomainEvent(User.UserId, accessId, AccessList));
            }
        }

        public void AddAccess(Guid accessId,Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.Now;
            AccessList.Add(accessId);
            AddDomainEvent(new UserAccessAddedDomainEvent(User.UserId, accessId, AccessList));
        }

        public void RemoveRole(Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.Now;
            AddDomainEvent(new UserPermissionsRemovedDomainEvent(this));
        }
        
        public void UpdateRole(Role newRole, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.Now;
            var previousRole = Role;
            Role = newRole;
            AddDomainEvent(new UserPermissionsChangedDomainEvent(this, previousRole));
        }
    }
}