﻿using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class DepartmentDTO
    {
        public Guid DepartmentId { get; set; }

        public string Name { get; set; }

        public string CostCenterId { get; set; }

        public string Description { get; set; }

        public Guid? ParentDepartmentId { get; set; }
        public IList<ManagedBy> ManagedBy { get; set; }

    }
}
