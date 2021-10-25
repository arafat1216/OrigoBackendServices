using System.Collections.Generic;
using System.Text.Json.Serialization;
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

        [JsonIgnore]
        public IReadOnlyCollection<PermissionSet> PermissionSets { get; protected set; }
    }
}