using System;

namespace OrigoApiGateway.Models.Asset
{
    public class OrigoAssetCounterDepartment
    {
        public OrigoAssetCounter Personal { get; set; }
        public OrigoAssetCounter NonPersonal { get; set; }
        public Guid DepartmentId { get; set; }

    }
}
