namespace HardwareServiceOrderServices.ServiceModels
{
    public class HardwareServiceOrderDTO
    {
        public DeliveryAddressDTO DeliveryAddress { get; set; }
        
        // TODO: This should be renamed
        public string ErrorDescription { get; set; }

        // TODO: Should this be renamed?
        public ContactDetailsExtendedDTO OrderedBy { get; set; }

        public AssetInfoDTO AssetInfo { get; set; }
    }
}
