using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrderServices.Infrastructure
{
    public class HardwareServiceOrderRepository : IHardwareServiceOrderRepository
    {
        public CustomerSettings ConfigureLoanPhoneAsync(Guid customerId, string loanPhoneNumber, string loanPhoneEmain)
        {
            throw new NotImplementedException();
        }

        public CustomerSettings ConfigureServiceIdAsync(Guid customerId, string serviceId)
        {
            throw new NotImplementedException();
        }

        public CustomerSettings GetSettingsAsync(Guid customerId)
        {
            throw new NotImplementedException();
        }
    }
}
