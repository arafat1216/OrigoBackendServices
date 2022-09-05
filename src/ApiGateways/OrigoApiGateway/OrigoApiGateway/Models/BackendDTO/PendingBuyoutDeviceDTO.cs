namespace OrigoApiGateway.Models.BackendDTO
{
    public class PendingBuyoutDeviceDTO
    {
        /// <summary>
        /// The ID of the Asset
        /// </summary>
        public Guid AssetLifeCycleId { get; set; }
        /// <summary>
        /// The last Working Day of the Owner
        /// </summary>
        public DateTime LasWorkingDay { get; set; }
        /// <summary>
        /// Ownder's email information to notify
        /// </summary>
        public EmailPersonAttributeDTO User { get; set; }
        /// <summary>
        /// ID of the user making request
        /// </summary>
        public Guid CallerId { get; set; }
        /// <summary>
        /// Role of the user making request
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// Name of the user making request if he is not the owner
        /// </summary>
        public string ManagerName { get; set; } = string.Empty;
        /// <summary>
        /// Currency for this customer
        /// </summary>
        public string Currency { get; set; }
    }
}
