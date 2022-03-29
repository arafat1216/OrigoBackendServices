using System;

namespace Asset.API.ViewModels
{
    public class UnAssignAssetToUser
    {
        public Guid DepartmentId { get; set; }
        public Guid CallerId { get; set; }
    }
}
