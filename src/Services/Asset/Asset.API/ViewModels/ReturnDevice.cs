using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asset.API.ViewModels
{
    public class ReturnDevice
    {
        /// <summary>
        /// A Assets ID (Guid), that will be made available.
        /// </summary>
        [Required]
        public Guid AssetLifeCycleId { get; set; }
        /// <summary>
        /// Email details of Asset User
        /// </summary>
        public EmailPersonAttribute? ContractHolder { get; set; }
        /// <summary>
        /// Email details of Dept Manager of the asset
        /// </summary>
        public IList<EmailPersonAttribute>? Managers { get; set; }
        /// <summary>
        /// Email details of Admins of the customer
        /// </summary>
        public IList<EmailPersonAttribute>? CustomerAdmins { get; set; }
        /// <summary>
        /// id of user making the endpoint call. Can be ignored by frontend.
        /// </summary>
        public Guid CallerId { get; set; }
        /// <summary>
        /// Id of the Return Location,to confirm pending returns.
        /// </summary>
        public Guid ReturnLocationId { get; set; }
    }
}
