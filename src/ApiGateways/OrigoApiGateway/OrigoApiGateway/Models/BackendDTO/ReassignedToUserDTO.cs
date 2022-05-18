using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class ReassignedToUserDTO
    {
        public bool Personal { get; } = true;
        public Guid UserId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid CallerId { get; set; }
        public EmailPersonAttributeDTO PreviousUser { get; set; }
        public EmailPersonAttributeDTO NewUser { get; set; }
        public IList<EmailPersonAttributeDTO> PreviousManagers { get; set; }
    }
}
