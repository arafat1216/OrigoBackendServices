using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareServiceOrderServices.ServiceModels
{
    public class HardwareServiceOrderLogDTO
    {
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
        public string ServiceProvider { get; set; }
    }
}
