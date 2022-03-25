using System;

namespace Asset.API.ViewModels
{
    public class AssignAssetToUser
    {
        public Guid UserId { get; set; }
        public Guid CallerId { get; set; }
    }
}
