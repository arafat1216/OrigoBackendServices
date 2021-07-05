using OrigoApiGateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IModuleServices
    {
        Task<IList<OrigoProductModule>> GetModulesAsync();
    }
}
