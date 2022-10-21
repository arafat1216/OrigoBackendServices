using System.ComponentModel.DataAnnotations;

namespace Customer.API.ViewModels
{
    public class UserPreference
    {
        /// <summary>
        ///     The user's language preference, using the <c>ISO 639-1</c> standard.
        /// </summary>
        [RegularExpression("^[a-z]{2}")] // Exactly 2 lowercase characters
        public string Language { get; set; }
        /// <summary>
        /// Is onboarding Asset Tile Closed by the User.
        /// </summary>
        public bool? IsAssetTileClosed { get; set; }
        /// <summary>
        /// Is onboarding Asset Tile Closed by the User.
        /// </summary>
        public bool? IsSubscriptionTileClosed { get; set; }
        /// <summary>
        /// Is the subscription handled related to the offboarding task.
        /// If true the user should not see the subscription offboarding tile.
        /// </summary>
        public bool? SubscriptionIsHandledForOffboarding { get; set; }
    }
}