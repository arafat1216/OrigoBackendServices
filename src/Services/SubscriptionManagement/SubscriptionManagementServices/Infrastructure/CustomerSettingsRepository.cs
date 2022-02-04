using SubscriptionManagementServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManagementServices.Infrastructure
{
    public class CustomerSettingsRepository : ICustomerSettingsRepository
    {
        public CustomerSettingsRepository(SubscriptionManagementContext subscriptionManagementContext)
        {
            SubscriptionManagementContext = subscriptionManagementContext;
        }

        public SubscriptionManagementContext SubscriptionManagementContext { get; }
    }
}
