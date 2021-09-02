using System.Collections.Generic;
using Common.Seedwork;

namespace CustomerServices.Models
{
    public sealed class Permission : Entity
    {
        public Permission(int id, string name)
        {
            Id = id;
            Name = name;
        }

        private Permission(){}

        public string Name { get; protected set; }

        public IReadOnlyCollection<PermissionSet> PermissionSets { get; protected set; }
    }
}