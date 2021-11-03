using System.Collections.Generic;

namespace AssetServices.Models
{
    public class AssetImei
    {
        protected AssetImei()
        {

        }

        public AssetImei(long imei)
        {
            Imei = imei;
        }

        public long Imei { get; set; }
    }
}
