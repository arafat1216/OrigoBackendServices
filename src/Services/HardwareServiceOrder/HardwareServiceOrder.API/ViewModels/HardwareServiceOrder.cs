namespace HardwareServiceOrder.API.ViewModels
{
    public class HardwareServiceOrder
    {
        public Guid Id { get; set; }
        public string FaultType { get; set; }
        public string BasicDescription { get; set; }
        public string UserDescription { get; set; }
        public Guid AssetLifecycleId { get; set; }
        public Location DeliveryAddress { get; set; }
    }
}
