﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.ServiceModels
{
    public record ManagedByDTO
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
    }
}
