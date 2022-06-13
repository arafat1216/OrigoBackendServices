using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class ReturnDeviceDTO
    {
        public Guid AssetLifeCycleId { get; set; }
        public EmailPersonAttributeDTO ContractHolder { get; set; }
        public IList<EmailPersonAttributeDTO> Managers { get; set; }
        public IList<EmailPersonAttributeDTO> CustomerAdmins { get; set; }
        public bool IsConfirm { get; set; } = false;
        public Guid CallerId { get; set; }
    }
}
