using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareServiceOrderServices.Infrastructure
{
    public class HardwareServiceOrderRepository : IHardwareServiceOrderRepository
    {
        private readonly HardwareServiceOrderContext _hardwareServiceOrderContext;
        public HardwareServiceOrderRepository(HardwareServiceOrderContext hardwareServiceOrderContext)
        {
            _hardwareServiceOrderContext = hardwareServiceOrderContext;
        }

        public async Task<CustomerSettings> ConfigureLoanPhoneAsync(Guid customerId, string loanPhoneNumber, string loanPhoneEmail, Guid callerId)
        {
            var settings = await GetSettingsAsync(customerId);
            if (settings == null)
            {
                var newSettings = new CustomerSettings(customerId, loanPhoneNumber, loanPhoneEmail, callerId);
                _hardwareServiceOrderContext.Add(newSettings);
                await _hardwareServiceOrderContext.SaveChangesAsync();
                return newSettings;
            }

            settings.LoanDevicePhoneNumber = loanPhoneNumber;
            settings.LoanDeviceEmail = loanPhoneEmail;
            _hardwareServiceOrderContext.Entry(settings).State = EntityState.Modified;
            await _hardwareServiceOrderContext.SaveChangesAsync();
            return settings;
        }

        public async Task<CustomerSettings> ConfigureServiceIdAsync(Guid customerId, string serviceId, Guid callerId)
        {
            var settings = await GetSettingsAsync(customerId);
            if (settings == null)
            {
                var newSettings = new CustomerSettings(customerId, serviceId, callerId);
                _hardwareServiceOrderContext.Add(newSettings);
                await _hardwareServiceOrderContext.SaveChangesAsync();
                return newSettings;
            }

            settings.ServiceId = serviceId;
            _hardwareServiceOrderContext.Entry(settings).State = EntityState.Modified;
            await _hardwareServiceOrderContext.SaveChangesAsync();
            return settings;
        }

        public async Task<CustomerSettings> GetSettingsAsync(Guid customerId)
        {
            return await _hardwareServiceOrderContext.CustomerSettings.FirstOrDefaultAsync(m => m.CustomerId == customerId);
        }
    }
}
