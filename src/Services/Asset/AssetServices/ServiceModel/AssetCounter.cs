

namespace AssetServices.ServiceModel
{
    public class AssetCounter
    {
        public int InUse { get; set; }
        public int InputRequired { get; set; }
        public int Active { get; set; }
        public int Available { get; set; }
        public int Expired { get; set; }
    }
}
