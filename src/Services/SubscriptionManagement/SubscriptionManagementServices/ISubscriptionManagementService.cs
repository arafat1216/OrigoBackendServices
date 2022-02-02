﻿
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices
{
    public interface ISubscriptionManagementService
    {
        Task<IList<string>> GetAllOperators();
        Task<Operator> GetOperator(string operatorName);
        Task<IList<string>> GetAllOperatorsForCustomer(Guid customerId);
        Task<bool> AddOperatorForCustomerAsync(Guid customerId, IList<string> operators);
        Task<bool> DeleteOperatorForCustomerAsync(Guid customerId, string operatorName);
        Task<bool> AddSubscriptionForCustomerAsync(Guid customerId);
    }
}
