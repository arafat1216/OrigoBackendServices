using System.Collections.Generic;
using Common.Seedwork;

namespace CustomerServices.Models
{
    public class Role : Entity
    {
        public Role(string name)
        {
            Name = name;
        }
        public string Name { get; protected set; }
        public IReadOnlyCollection<PermissionSet> GrantedPermissions { get; protected set; }
    }
}
