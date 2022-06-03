namespace HardwareServiceOrder.API.ViewModels
{
    public class HardwareServiceOrder
    {
        public Location DeliveryAddress { get; set; }
        public string ErrorDescription { get; set; }
        public OrderedByUserDTO OrderedBy { get; set; }
        public AssetInfo AssetInfo { get; set; }

    }
}
