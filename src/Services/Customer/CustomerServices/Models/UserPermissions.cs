using System;
using System.Collections.Generic;
using Common.Seedwork;

namespace CustomerServices.Models
{
    public class UserPermissions : Entity
    {
        public UserPermissions(User user, Role role, IReadOnlyCollection<Guid> accessList)
        {
            User = user;
            Role = role;
            AccessList = accessList;
        }

        protected UserPermissions()
        {
        }

        public User User { get; set; }
        public Role Role { get; protected set; }

        public IReadOnlyCollection<Guid> AccessList { get; protected set; }
    }
}