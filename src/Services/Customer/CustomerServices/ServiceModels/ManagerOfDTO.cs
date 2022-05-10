using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.ServiceModels
{
    public record ManagerOfDTO
    {
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
}
