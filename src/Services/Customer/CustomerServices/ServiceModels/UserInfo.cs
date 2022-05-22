using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.ServiceModels
{
    public class UserInfo
    {
        public string UserName { get; set; }
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid DepartmentId { get; set; }

    }
}
