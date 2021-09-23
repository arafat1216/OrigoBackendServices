using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common.Seedwork;

namespace CustomerServices.Models
{
    public class UserPermissions : Entity
    {
        public UserPermissions(User user, Role role, IList<Guid> accessList)
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
        IList<Guid> accessList;
        public IList<Guid> AccessList { get { return new ReadOnlyCollection<Guid>(accessList); } protected set { accessList = value; } }
    }
}