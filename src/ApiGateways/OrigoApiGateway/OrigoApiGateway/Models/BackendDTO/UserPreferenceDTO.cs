namespace OrigoApiGateway.Models.BackendDTO
{
    public class UserPreferenceDTO
    {
        public string Language { get; set; }
        /// <summary>
        /// Is onboarding Asset Tile Closed by the User.
        /// </summary>
        public bool? IsAssetTileClosed { get; set; }
        /// <summary>
        /// Is onboarding Asset Tile Closed by the User.
        /// </summary>
        public bool? IsSubscriptionTileClosed { get; set; }
    }
}
