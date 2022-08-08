using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.Services;

namespace HardwareServiceOrderServices
{
    /// <summary>
    ///     Indicates that the created class will execute necessary operations based on different service-order statuses
    /// </summary>
    public interface IStatusHandlerFactory
    {
        /// <summary>
        ///     Creates and Retrieves the implementation of <see cref="ServiceOrderStatusHandlerService"/> that will be
        ///     used to handle service-order statuses for a particular <see cref="ServiceTypeEnum"/>.
        /// </summary>
        /// <param name="serviceType">Different Service types <see cref="ServiceTypeEnum"/></param>
        ///
        /// <exception cref="NotSupportedException"> Thrown if the provided <paramref name="serviceType"/> is
        ///     invalid/unsupported</exception>
        public ServiceOrderStatusHandlerService GetStatusHandler(ServiceTypeEnum serviceType);
    }
}
