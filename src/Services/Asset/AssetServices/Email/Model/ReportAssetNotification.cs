using System.Collections.Generic;

namespace AssetServices.Email.Model
{
    public class ReportAssetNotification
    {
        /// <summary>
        /// First name of the email recipient
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Type of the report
        /// </summary>
        public string ReportType { get; set; }
        /// <summary>
        /// Name of the asset
        /// </summary>
        public string AssetName { get; set; }
        /// <summary>
        /// Id of the asset
        /// </summary>
        public string AssetId { get; set; }
        /// <summary>
        /// Date of the Report
        /// </summary>
        public string ReportDate { get; set; }
        /// <summary>
        /// who reported it
        /// </summary>
        public string ReportedBy { get; set; }
        /// <summary>
        /// Description of the rpeort
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Start time of the incident
        /// </summary>
        public string DateFrom { get; set; }
        /// <summary>
        /// End time of the incident
        /// </summary>
        public string DateTo { get; set; }
        /// <summary>
        /// Address of the incident
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Email address of the recipient
        /// </summary>
        public IList<string> Recipients { get; set; }
        /// <summary>
        /// Subject of the email
        /// </summary>
        public string Subject { get; set; } = "Asset Reported Stolen/Lost";
        public const string TemplateName = "ReportAsset";
    }
}
