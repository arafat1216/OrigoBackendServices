using OrigoApiGateway.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IPartnerServices
    {
        Task<Partner> CreatePartnerAsync(Guid organizationId, Guid callerId);
        Task<Partner> GetPartnerAsync(Guid partnerId);
        Task<IList<Partner>> GetPartnersAsync();
    }
}
