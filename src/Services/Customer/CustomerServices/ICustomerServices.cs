using CustomerServices.Models;
using System.Collections.Generic;

namespace CustomerServices
{
    public interface ICustomerServices
    {
        IList<Customer> GetCustomers();
    }
}