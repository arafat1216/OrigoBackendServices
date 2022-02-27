using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices;

public interface IOperatorService
{
    Task<IList<OperatorDTO>> GetAllOperatorsAsync();
    Task<OperatorDTO?> GetOperatorAsync(int id);
}