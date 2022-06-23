using System;

namespace Asset.API.ViewModels
{
    public class AssignAssetToUser
    {
        public Guid UserId { get; set; } = Guid.Empty;
        public Guid DepartmentId { get; set; } = Guid.Empty;
        public Guid UserAssigneToDepartment { get; set; } = Guid.Empty;

        public Guid CallerId { get; set; }
    }
}
