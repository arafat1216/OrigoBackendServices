using System;

namespace Asset.API.ViewModels
{
    public class AssetCounterDepartment
    {
        public AssetCounter? Personal { get; set; }
        public AssetCounter? NonPersonal { get; set; }
        public Guid? DepartmentId { get; set; }

    }
}
