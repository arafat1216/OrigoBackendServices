﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models
{
    public class UpdateCustomerLabelsData
    {
        public IList<Label> Labels { get; set; }
        public Guid CallerId { get; set; }
    }
}
