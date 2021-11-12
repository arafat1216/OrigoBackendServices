using Common.Seedwork;

namespace AssetServices.Models
{
    public class AssetImei : Entity
    {
        protected AssetImei() { }

        public AssetImei(long imei)
        {
            Imei = imei;
        }

        public long Imei { get; set; }
    }
}
