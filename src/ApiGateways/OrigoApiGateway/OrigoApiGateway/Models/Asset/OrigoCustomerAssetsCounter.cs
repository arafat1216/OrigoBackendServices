using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.Asset
{
    public class OrigoCustomerAssetsCounter
    {

        public OrigoAssetCounter Personal { get; set; }
        public OrigoAssetCounter NonPersonal { get; set; }
        public IList<OrigoAssetCounterDepartment>? Departments { get; set; } = new List<OrigoAssetCounterDepartment>();
        public int UsersPersonalAssets { get; set; }
        public Guid OrganizationId { get; set; }
    }
}
