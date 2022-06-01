namespace OrigoApiGateway.Models.HardwareServiceOrder
{
    public enum RecipientTypeEnum
    {
        /// <summary>
        ///     The delivery is to a customer/organization address.
        /// </summary>
        Organization = 1,

        /// <summary>
        ///     The delivery is for a user's personal/private/home address.
        /// </summary>
        Personal = 2
    }
}
