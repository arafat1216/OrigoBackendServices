using System;
using System.Collections.Generic;

namespace AssetServices.ServiceModel
{
    public class AssignAssetDTO
    {
        public bool Personal { get; set; }
        public Guid UserId { get; set; } = Guid.Empty;
        public EmailPersonAttributeDTO? PreviousUser { get; set; }
        public EmailPersonAttributeDTO? NewUser { get; set; }
        public IList<EmailPersonAttributeDTO>? PreviousManagers { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid UserAssigneToDepartment { get; set; }

        public Guid CallerId { get; set; }
    }
}
