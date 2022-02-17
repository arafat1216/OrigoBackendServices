using Common.Seedwork;
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
        }

        public string DataPackageName { get; set; }
        public virtual ICollection<SubscriptionOrder>? SubscriptionOrders { get; set; }
    }
}
