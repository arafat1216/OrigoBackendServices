using Common.Seedwork;
namespace SubscriptionManagementServices.Models
{
    public class Datapackage : Entity
    {
        public Datapackage()
        {

        }
        public Datapackage(string datapackageName, Guid callerId)
        {
            DatapackageName = datapackageName;
            CreatedBy = callerId;
            UpdatedBy = callerId;
        }

        public string DatapackageName { get; set; }
        public virtual ICollection<SubscriptionOrder>? SubscriptionOrders { get; set; }
    }
}
