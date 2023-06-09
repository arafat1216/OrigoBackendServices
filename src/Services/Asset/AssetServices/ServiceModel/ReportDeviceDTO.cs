﻿using Common.Enums;
using System;
using System.Collections.Generic;

namespace AssetServices.ServiceModel
{
    public class ReportDeviceDTO
    {
        public Guid AssetLifeCycleId { get; set; }
        public EmailPersonAttributeDTO? ContractHolderUser { get; set; }
        public IList<EmailPersonAttributeDTO>? Managers { get; set; }
        public IList<EmailPersonAttributeDTO>? CustomerAdmins { get; set; }
        public ReportCategory ReportCategory { get; set; }
        public string ReportedBy { get; set; }
        public string Description { get; set; }
        public DateTime TimePeriodFrom { get; set; }
        public DateTime TimePeriodTo { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public Guid CallerId { get; set; }
    }
}
