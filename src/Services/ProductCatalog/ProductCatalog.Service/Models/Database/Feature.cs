using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Service.Models.Database
{
    public class Feature : Entity
    {
        public int Id { get; set; }

        public int FeatureTypeId { get; set; }

        public string AccessControlPermissionNode { get; set; }
    }
}
