using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.Email.Model
{
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
        /// Subject of the email
        /// </summary>
        public string Subject { get; set; } = "Asset Buyout";
        public const string TemplateName = "ManagerOnBehalfBuyout";
    }
}
