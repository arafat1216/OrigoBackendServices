using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Common.Seedwork;

namespace CustomerServices.Models
{
    public sealed class Permission : Entity
    {
        public Permission(int id, string name, DateTime createdDate)
        {
            Id = id;
            Name = name;
            CreatedDate = createdDate;
        }

        private Permission(){}

        public string Name { get; protected set; }

        [JsonIgnore]
        public IReadOnlyCollection<PermissionSet> PermissionSets { get; protected set; }
    }
}