using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.Email.Model
{
    /// <summary>
    /// Email Data to notify User when Manager perform buyout on behalf
    /// </summary>
    public class ManagerOnBehalfBuyoutNotification
    {
        /// <summary>
        /// Username of the asset owner
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Manager who executed on behalf of the user
        /// </summary>
        public string ManagerName { get; set; }
        /// <summary>
        /// Asset Id
        /// </summary>
        public string AssetId { get; set; }
        /// <summary>
        /// Asset Name
        /// </summary>
        public string AssetName { get; set; }
        /// <summary>
        /// Asset Buyout Date
        /// </summary>
        public string BuyoutDate { get; set; }
        /// <summary>
        /// Asset Buyout Price
        /// </summary>
        public string BuyoutPrice { get; set; }
        /// <summary>
        /// Currency of the price
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// Email address of the recipient
        /// </summary>
        public string Recipient { get; set; }
        /// <summary>
        /// Name of the subject template in resource
        /// </summary>
        public const string SubjectTemplatename = "ManagerOnBehalfBuyout_Subject";
        /// <summary>
        /// Name of the email body template in resource
        /// </summary>
        public const string TemplateName = "ManagerOnBehalfBuyout";
    }
}
