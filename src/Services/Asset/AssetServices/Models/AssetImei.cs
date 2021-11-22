namespace AssetServices.Models
{
    public class AssetImei
    {
        protected AssetImei() { }

        public AssetImei(long imei)
        {
            Imei = imei;
        }
        public int Id { get; set; }

        public long Imei { get; set; }
    }
}
