using System.ComponentModel.DataAnnotations;

namespace CustomerServices.ServiceModels
{
    public record UserPreferenceDTO
    {
        /// <summary>
        ///     Backing field for <see cref="Language"/>
        /// </summary>
        private string _language;

        /// <summary>
        ///     The organizations language preference, using the lowercase <c>ISO 639-1</c> standard.
        /// </summary>
        /// <example> en </example>
        [RegularExpression("^[a-zA-Z]{2}")] // Exactly 2 characters
        public string Language
        {
            get { return _language; }
            init { _language = value.ToLower(); }
        }
        /// <summary>
        /// Is onboarding Asset Tile Closed by the User.
        /// </summary>
        public bool IsAssetTileClosed { get; set; }
        /// <summary>
        /// Is onboarding Asset Tile Closed by the User.
        /// </summary>
        public bool IsSubscriptionTileClosed { get; set; }
        /// <summary>
        /// Is the subscription handled related to the offboarding task.
        /// If true the user should not see the subscription offboarding tile.
        /// </summary>
        public bool SubscriptionIsHandledForOffboarding { get; set; }
    }
}