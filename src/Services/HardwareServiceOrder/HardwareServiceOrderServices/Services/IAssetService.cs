using Common.Enums;
using HardwareServiceOrderServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareServiceOrderServices.Services
{
    /// <summary>
    /// Generic interface for handling asset related service
    /// </summary>
    public interface IAssetService
    {
        /// <summary>
        /// Update the status of an asset life cycle
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="assetLifeCycleId">Asset life-cycle identifier</param>
        /// <param name="status">New status of the asset life-cycle</param>
        /// <returns></returns>
        [Obsolete]
        Task UpdateAssetLifeCycleStatusAsync(Guid customerId, Guid assetLifeCycleId, AssetLifecycleStatus status);


        /// <summary>
        ///     Connects to the Asset-microservice, notifying it about new and updated service-statuses.
        /// </summary>
        /// <param name="assetLifecycleId"> The ID for the asset-lifecycle. </param>
        /// <param name="newServiceStatus"> The new service-status. </param>
        /// <param name="newImeis"> If the device has been replaced, a list of the new IMEI numbers. </param>
        /// <param name="newSerialNumber"> If the device has been replaced, the new serial-number. </param>
        /// <returns> The awaitable task. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="newServiceStatus"/> has a <see langword="null"/> equivalent. </exception>
        /// <exception cref="NotImplementedException"> Thrown when a not-supported, or not implemented (out of scope) service-status is triggered. </exception>
        /// <exception cref="HttpRequestException"> Thrown when we fail to update the asset microservice. </exception>
        Task UpdateAssetLifeCycleStatusAsync(Guid assetLifecycleId, ServiceStatusEnum newServiceStatus, IEnumerable<string>? newImeis, string? newSerialNumber);
    }
}
