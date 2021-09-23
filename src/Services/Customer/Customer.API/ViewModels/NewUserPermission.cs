using Common.Enums;
using System;
using System.Collections.Generic;

namespace Customer.API.ViewModels
{
    public class NewUserPermission
    {
        public PredefinedRole Role { get; set; }
        public IList<Guid> AccessList { get; set; }
    }
}