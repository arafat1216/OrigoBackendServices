namespace SubscriptionManagementServices.Models
{
    public interface IOperatorRepository
    {
        Task<IList<Operator>> GetAllOperatorsAsync();
        Task<Operator?> GetOperatorAsync(int id);
    }
}
