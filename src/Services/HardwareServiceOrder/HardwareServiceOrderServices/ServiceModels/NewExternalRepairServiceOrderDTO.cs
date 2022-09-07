namespace HardwareServiceOrderServices.ServiceModels
{
    /// <summary>
    ///     Contains the information required for registering a new repair-order with an external service-provider.
    /// </summary>
    public class NewExternalRepairServiceOrderDTO : NewExternalServiceOrderDTO
    {
        /// <summary>
        ///     Reserved JSON (de-)serializer constructor.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public NewExternalRepairServiceOrderDTO() : base()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {

        }

        public NewExternalRepairServiceOrderDTO(Guid userId,
                                         string firstName,
                                         string lastName,
                                         string? phoneNumber,
                                         string email,
                                         Guid organizationId,
                                         string organizationName,
                                         string? organizationNumber,
                                         Guid partnerId,
                                         string partnerName,
                                         string partnerOrganizationNumber,
                                         DeliveryAddressDTO deliveryAddress,
                                         AssetInfoDTO assetInfo,
                                         string errorDescription,
                                         ISet<string>? includedExternalAddonIds) 
                                         : base(userId, firstName, lastName, phoneNumber, email, organizationId, organizationName, organizationNumber, partnerId, partnerName, partnerOrganizationNumber, assetInfo, includedExternalAddonIds)
        {
            DeliveryAddress = deliveryAddress;
            ErrorDescription = errorDescription;
        }

        /// <summary>
        ///     The address that the asset should be shipped to once the service is complete.
        /// </summary>
        [Required]
        public DeliveryAddressDTO DeliveryAddress { get; set; }

        /// <summary>
        ///     The user's description of what's wrong with the device.
        /// </summary>
        /// <example> I dropped the device, and now it has a broken screen and has problems charging. </example>
        [Required]
        public string ErrorDescription { get; set; }

    }
}
