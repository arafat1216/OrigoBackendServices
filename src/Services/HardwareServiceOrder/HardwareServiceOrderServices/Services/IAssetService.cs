using Common.Enums;
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
        Task UpdateAssetLifeCycleStatusAsync(Guid customerId, Guid assetLifeCycleId, AssetLifecycleStatus status);
    }
}
