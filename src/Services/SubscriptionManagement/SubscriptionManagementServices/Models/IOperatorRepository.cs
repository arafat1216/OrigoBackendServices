namespace SubscriptionManagementServices.Models
{
    public interface IOperatorRepository
    {
        Task<IList<Operator>> GetAllOperatorsAsync(bool asNoTracking);
        Task<Operator?> GetOperatorAsync(int id);
    }
}
