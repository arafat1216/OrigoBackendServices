namespace HardwareServiceOrderServices.Models
{
    public interface IHardwareServiceOrderRepository
    {
        CustomerSettings ConfigureServiceIdAsync(Guid customerId, string serviceId);
        CustomerSettings ConfigureLoanPhoneAsync(Guid customerId, string loanPhoneNumber, string loanPhoneEmain);
        CustomerSettings GetSettingsAsync(Guid customerId);
    }
}
