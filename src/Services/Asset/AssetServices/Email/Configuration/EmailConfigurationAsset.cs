using Common.Configuration;

namespace AssetServices.Email.Configuration
{
    public record EmailConfigurationAsset : EmailConfiguration
    {
        /// <summary>
        /// Link to an spesific asset.
        /// </summary>
        public string AssetPath { get; set; }

    }
}
