using System;

namespace Asset.API.ViewModels
{
    public class ReAssignAsset
    {
        public bool Personal { get; set; }
        public Guid UserId { get; set; } = Guid.Empty;
        public Guid DepartmentId { get; set; }
        public Guid CallerId { get; set; }
    }
}
