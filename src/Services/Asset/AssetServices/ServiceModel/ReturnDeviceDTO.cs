using System;
using System.Collections.Generic;

namespace AssetServices.ServiceModel
{
    public class ReturnDeviceDTO
    {
        public Guid AssetLifeCycleId { get; init; }
        public EmailPersonAttributeDTO? ContractHolder { get; init; }
        public IList<EmailPersonAttributeDTO>? Managers { get; init; }
        public IList<EmailPersonAttributeDTO>? CustomerAdmins { get; init; }
        public Guid CallerId { get; set; }
    }
}
