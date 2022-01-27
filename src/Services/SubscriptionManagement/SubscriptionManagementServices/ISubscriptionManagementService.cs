using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManagementServices
{
    public interface ISubscriptionManagementService
    {
        Task<string> GetOperator();
    }
}
