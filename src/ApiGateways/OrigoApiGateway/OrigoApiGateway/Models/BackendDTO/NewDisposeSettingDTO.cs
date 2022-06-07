using System;
using System.ComponentModel.DataAnnotations;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class NewDisposeSettingDTO
    {
        public string PayrollContactEmail { get; set; }
        public Guid CallerId { get; set; }

    }
}
