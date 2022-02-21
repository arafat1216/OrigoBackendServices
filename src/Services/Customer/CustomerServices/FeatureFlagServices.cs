using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerServices.Infrastructure;
using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerServices
{
    public class FeatureFlagServices : IFeatureFlagServices
    {
        private readonly CustomerContext _customerContext;

        public FeatureFlagServices(CustomerContext customerContext)
        {
            _customerContext = customerContext;
        }

        public async Task<IList<string>> GetFeatureFlags()
        {
            var featureFlags = await _customerContext.FeatureFlags.Where(f => !f.CustomerId.HasValue).ToListAsync();
            return featureFlags.Select(f => f.Name).ToList();
        }

        public async Task<IList<string>> GetFeatureFlags(Guid customerId)
        {
            var featureFlags = await _customerContext.FeatureFlags.Where(f => f.CustomerId == customerId || !f.CustomerId.HasValue).ToListAsync();
            return featureFlags.Select(f => f.Name).ToList();
        }

        public async Task AddFeatureFlags(string featureFlagName, Guid? customerId)
        {
            await _customerContext.FeatureFlags.AddAsync(new FeatureFlag()
            {
                CustomerId = customerId, Name = featureFlagName
            });
            await _customerContext.SaveChangesAsync();
        }

        public async Task DeleteFeatureFlags(string featureFlagName, Guid? customerId)
        {
            var featureFlag = await _customerContext.FeatureFlags.FirstOrDefaultAsync(f => f.Name == featureFlagName && f.CustomerId == customerId);
            _customerContext.FeatureFlags.Remove(featureFlag);
            await _customerContext.SaveChangesAsync();
        }
    }
}