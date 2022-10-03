using System.Collections.Generic;

namespace AssetServices.Email.Model
{
    public class UnassignedFromManagerNotification
    {
        /// <summary>
        /// Email address of the recipients
        /// </summary>
        public IList<string> Recipient { get; set; }
        /// <summary>
        /// Subject of the email
        /// </summary>
        public const string Subject = "UnassignedFromManager_Subject";
        public const string TemplateName = "UnassignedFromManager";

    }
}
