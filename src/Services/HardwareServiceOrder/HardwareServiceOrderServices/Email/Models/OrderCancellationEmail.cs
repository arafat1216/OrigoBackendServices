using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareServiceOrderServices.Email.Models
{
    public class OrderCancellationEmail
    {
        /// <summary>
        /// ID of the customer
        /// </summary>
        public Guid CustomerId { get; set; }
        /// <summary>
        /// Unique identifier of the order
        /// </summary>
        public Guid OrderId { get; set; }
        /// <summary>
        /// First name of the email recipient
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Name of the asset
        /// </summary>
        public string AssetName { get; set; }
        /// <summary>
        /// Unique identifier of the asset
        /// </summary>
        public Guid AssetId { get; set; }
        /// <summary>
        /// Date of the order
        /// </summary>
        public DateTimeOffset OrderDate { get; set; }
        /// <summary>
        /// Type of the repair
        /// </summary>
        public string RepairType { get; set; }
        /// <summary>
        /// Category of the fault
        /// </summary>
        public string FaultCategory { get; set; }
        /// <summary>
        /// Email address of the recipient
        /// </summary>
        public string Recipient { get; set; }
        /// <summary>
        /// Subject of the email
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// Origo order link
        /// </summary>
        public string OrderLink { get; set; }
        public const string TemplateKeyName = "OrderCancellationEmail";
        public const string SubjectKeyName = "OrderCancellationEmail_Subject";
    }
}
