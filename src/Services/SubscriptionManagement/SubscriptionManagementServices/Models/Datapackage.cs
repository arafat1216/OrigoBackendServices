using Common.Seedwork;
namespace SubscriptionManagementServices.Models
{
    public class Datapackage : Entity
    {
        public Datapackage(string datapackageName)
        {
            DatapackageName = datapackageName;
        }

        public string DatapackageName { get; set; }
       
    }
}
