using Common.Seedwork;
using System.ComponentModel.DataAnnotations;

namespace SubscriptionManagementServices.Models
{
    public class DataPackage : Entity
    {
        public DataPackage()
        {

        }
        public DataPackage(string dataPackageName, Guid callerId)
        {
            DataPackageName = dataPackageName;
            CreatedBy = callerId;
            UpdatedBy = callerId;
            CreatedDate = DateTime.UtcNow;
        }
        [MaxLength(50)]
        public string DataPackageName { get; set; }
        public virtual ICollection<CustomerSubscriptionProduct>? CustomerSubscriptionProducts { get; set; }
    }
}
