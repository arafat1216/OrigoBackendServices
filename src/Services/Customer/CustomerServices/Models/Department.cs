using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Seedwork;

namespace CustomerServices.Models
{
    public class Department : Entity
    {
        public string Name { get; protected set; }

        public string CostCenterId { get; set; }

        public Department ParentDepartment { get; protected set; }

        public IReadOnlyCollection<User> Users { get; set; }
    }
}
