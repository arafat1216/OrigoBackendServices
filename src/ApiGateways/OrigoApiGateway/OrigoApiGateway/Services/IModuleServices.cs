using OrigoApiGateway.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IModuleServices
    {
        Task<IList<OrigoProductModule>> GetModulesAsync(Guid? customerId);
    }
}
