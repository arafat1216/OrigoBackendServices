using System;

namespace AssetServices.ServiceModel
{
    public class AssetCounterDepartment
    {
        public AssetCounter Personal { get; set; } = new AssetCounter();
        public AssetCounter NonPersonal { get; set; } = new AssetCounter();
        public Guid? DepartmentId { get; set; }

    }
}
