﻿using Common.Enums;
using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class ReportDeviceDTO
    {
        public Guid AssetLifeCycleId { get; set; }

        public EmailPersonAttributeDTO ContractHolderUser { get; set; }

        public IList<EmailPersonAttributeDTO> Managers { get; set; }

        public IList<EmailPersonAttributeDTO> CustomerAdmins { get; set; }

        public ReportCategory ReportCategory { get; set; }

        public string ReportedBy { get; set; }

        public string Description { get; set; }

        public DateTime TimePeriodFrom { get; set; }

        public DateTime TimePeriodTo { get; set; }

        // TODO: Should this be enforced to only accept the 2-character ISO code like we use in other places?
        public string Country { get; set; }

        public string Address { get; set; }

        public string PostalCode { get; set; }

        public string City { get; set; }

        public Guid CallerId { get; set; }
    }
}
