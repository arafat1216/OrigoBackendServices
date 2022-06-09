namespace HardwareServiceOrder.API.ViewModels
{
    public class NewHardwareServiceOrder
    {
        public DeliveryAddress DeliveryAddress { get; set; }
        public string ErrorDescription { get; set; }
        public ContactDetails OrderedBy { get; set; }
        public AssetInfo AssetInfo { get; set; }
    }
}
