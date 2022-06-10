using System;
using System.Collections.Generic;

namespace Asset.API.ViewModels
{
    public class CustomerAssetsCounter
    {
        public AssetCounter? Personal { get; set; }
        public AssetCounter? NonPersonal { get; set; }
        public IList<AssetCounterDepartment>? Departments { get; set; }
        public int UsersPersonalAssets { get; set; }
        public Guid? OrganizationId { get; set; }
    }
}
