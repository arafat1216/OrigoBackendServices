﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models
{
    public class DeleteCustomerLabelsData
    {
        public IList<Guid> LabelGuids { get; set; }
        public Guid CallerId { get; set; }
    }
}