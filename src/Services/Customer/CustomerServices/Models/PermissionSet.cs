using System.Collections.Generic;
using Common.Seedwork;

namespace CustomerServices.Models
{
    public sealed class PermissionSet : Entity
    {
        public PermissionSet(int id, string name)
        {
            Id = id;
            Name = name;
        }

        private PermissionSet() { }

        public string Name { get; protected set; }
        public IReadOnlyCollection<Permission> Permissions { get; protected set; }

        public IReadOnlyCollection<Role> Roles { get; protected set; }
    }
}
