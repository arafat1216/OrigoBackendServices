using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IFeatureFlagServices
    {
        Task<IList<string>> GetFeatureFlags();
        Task<IList<string>> GetFeatureFlags(Guid customerId);
        Task AddFeatureFlags(string featureFlagName, Guid? customerId);
        Task DeleteFeatureFlags(string featureFlagName, Guid? customerId);
    }
}