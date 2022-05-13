using System;
using System.Collections.Generic;

namespace Asset.API.ViewModels
{
    public class ReAssignAsset
    {
        public bool Personal { get; set; }
        public Guid UserId { get; set; } = Guid.Empty;
        public EmailPersonAttribute? PreviousUser { get; set; }
        public EmailPersonAttribute? NewUser { get; set; }
        public IList<EmailPersonAttribute>? PreviousManagers { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid CallerId { get; set; }
    }
}
