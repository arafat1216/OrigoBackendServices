using Common.Enums;
using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Request object
    /// </summary>
    public class NewUserPermissions
    {
        public string Role { get; set; }
        public IList<Guid> AccessList { get; set; }
    }
}
