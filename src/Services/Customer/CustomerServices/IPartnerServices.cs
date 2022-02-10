using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices
{
    public interface IPartnerServices
    {
        Task<Partner> CreatePartnerAsync(Partner partner);
        Task<Partner> GetPartnerAsync(Guid partnerId);
        Task<IList<Partner>> GetPartnersAsync();
    }
}
