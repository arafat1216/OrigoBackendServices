using System;

namespace OrigoApiGateway.Models.Asset
{
    public class AssignAsset
    {
        public Guid UserId { get; set; } = Guid.Empty;
        public Guid DepartmentId { get; set; } = Guid.Empty;
    }
}
