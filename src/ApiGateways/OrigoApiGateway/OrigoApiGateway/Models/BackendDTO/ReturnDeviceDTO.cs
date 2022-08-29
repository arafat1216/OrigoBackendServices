using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class ReturnDeviceDTO
    {
        public Guid AssetLifeCycleId { get; set; }
        public IList<EmailPersonAttributeDTO> Managers { get; set; }
        public EmailPersonAttributeDTO User { get; set; }
        public bool IsConfirm { get; set; } = false;
        public Guid CallerId { get; set; }
        public Guid ReturnLocationId { get; set; }
        public string Role { get; set; }
    }
}
