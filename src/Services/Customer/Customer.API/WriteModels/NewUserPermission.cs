﻿using System;
using System.Collections.Generic;

namespace Customer.API.WriteModels
{
    public class NewUserPermission
    {
        public string Role { get; set; }
        public IList<Guid> AccessList { get; set; }
        public Guid CallerId { get; set; }
    }
}