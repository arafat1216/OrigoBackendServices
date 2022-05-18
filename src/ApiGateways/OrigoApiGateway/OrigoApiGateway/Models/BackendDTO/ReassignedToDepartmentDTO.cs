using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class ReassignedToDepartmentDTO
    {
        public bool Personal { get; } = false;
        public Guid DepartmentId { get; set; }
        public Guid CallerId { get; set; }
        public IList<EmailPersonAttributeDTO> PreviousManagers { get; set; }
    }
}
