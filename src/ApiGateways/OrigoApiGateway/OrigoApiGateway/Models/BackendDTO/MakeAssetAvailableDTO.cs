using System;
using System.ComponentModel.DataAnnotations;

namespace OrigoApiGateway.Models.BackendDTO
{
    /// <summary>
    /// A Assets ID (Guid), that will be made available.
    /// </summary>
    public class MakeAssetAvailableDTO
    {
        [Required]
        public Guid AssetLifeCycleId { get; set; }
        public EmailPersonAttributeDTO PreviousUser { get; set; }

        /// <summary>
        /// id of user making the endpoint call. Can be ignored by frontend.
        /// </summary>
        public Guid CallerId { get; set; }
    }
}
