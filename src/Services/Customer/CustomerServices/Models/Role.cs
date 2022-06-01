using System;
using System.Collections.Generic;
using Common.Seedwork;

namespace CustomerServices.Models
{
    public class Role : Entity
    {
        protected Role() { }
        public Role(int id, string name, DateTime createdDate)
        {
            Id = id;
            Name = name;
            CreatedDate = createdDate;
        }

        public Role(string name)
        {
            Name = name;
        }
        public string Name { get; protected set; }
        public IReadOnlyCollection<PermissionSet> GrantedPermissions { get; protected set; }
    }
}
