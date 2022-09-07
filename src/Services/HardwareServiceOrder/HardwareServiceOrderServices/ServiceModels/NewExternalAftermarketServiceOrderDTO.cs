namespace HardwareServiceOrderServices.ServiceModels
{
    internal class NewExternalAftermarketServiceOrderDTO : NewExternalServiceOrderDTO
    {
        /// <summary>
        ///     Reserved JSON (de-)serializer constructor.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public NewExternalAftermarketServiceOrderDTO() : base()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {

        }

        public NewExternalAftermarketServiceOrderDTO(Guid userId,
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
                                         AssetInfoDTO assetInfo,
                                         ISet<string>? includedExternalAddonIds)
                                         : base(userId, firstName, lastName, phoneNumber, email, organizationId, organizationName, organizationNumber, partnerId, partnerName, partnerOrganizationNumber, assetInfo, includedExternalAddonIds)
        {

        }


    }
}
