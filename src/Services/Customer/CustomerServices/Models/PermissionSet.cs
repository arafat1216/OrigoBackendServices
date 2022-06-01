using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Common.Seedwork;

namespace CustomerServices.Models
{
    public sealed class PermissionSet : Entity
    {
        public PermissionSet(int id, string name, DateTime createdDate)
        {
            Id = id;
            Name = name;
            CreatedDate = createdDate;
        }

        private PermissionSet() { }

        public string Name { get; protected set; }

        public IReadOnlyCollection<Permission> Permissions { get; protected set; }

        [JsonIgnore]
        public IReadOnlyCollection<Role> Roles { get; protected set; }
    }
}
