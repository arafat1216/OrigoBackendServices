using System;
using System.Collections.Generic;

namespace AssetServices.ServiceModel
{
    public class CustomerAssetsCounterDTO
    {
        public AssetCounter Personal { get; set; } = new AssetCounter();
        public AssetCounter NonPersonal { get; set; } = new AssetCounter();
        public IList<AssetCounterDepartment> Departments { get; set; } = new List<AssetCounterDepartment>();
        public int UsersPersonalAssets { get; set; }
        public Guid OrganizationId { get; set; }
    }
}
