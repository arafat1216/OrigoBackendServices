using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.ServiceModels
{
    public class TechstepCoreDataDTO
    {
        public long TechstepCustomerId { get; set; }
        public string OrgNumber { get; set; }
        public string Name { get; set; }
        public bool IsInactive { get; set; }
        public string AccountOwner { get; set; }
        public string ChainCode { get; set; }
        public string CountryCode { get; set; }
        public string MainCountryCode { get; set; }
        public int CustomerClassId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsBlocked { get; set; }
        public int ChainCount { get; set; }
    }
}
