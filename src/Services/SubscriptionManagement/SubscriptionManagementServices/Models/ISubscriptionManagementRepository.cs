using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManagementServices.Models
{
    public interface ISubscriptionManagementRepository
    {
        Task<Operator> GetOperatorAsync(string name);
    }
}
