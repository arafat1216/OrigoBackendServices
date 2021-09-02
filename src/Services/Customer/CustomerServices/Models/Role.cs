using System.Collections.Generic;
using Common.Seedwork;

namespace CustomerServices.Models
{
    public class Role : Entity
    {
        protected Role() { }
        public Role(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public Role(string name)
        {
            Name = name;
        }
        public string Name { get; protected set; }
        public IReadOnlyCollection<PermissionSet> GrantedPermissions { get; protected set; }
    }
}
