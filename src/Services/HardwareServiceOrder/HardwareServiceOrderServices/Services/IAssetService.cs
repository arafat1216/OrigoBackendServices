﻿using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrderServices.Services
{
    /// <summary>
    /// Generic interface for handling asset related service
    /// </summary>
    public interface IAssetService
    {
        /// <summary>
        /// Connects to the Asset-microservice, notifying it about new service-statuses.
        /// </summary>
        /// <param name="endpoint">Endpoint of the Asset-microservice</param>
        /// <param name="assetLifecycleId">The ID for the asset-lifecycle.</param>
        /// <param name="data">Request body which contains the necessary information to update Asset status</param>
        /// <returns></returns>
        Task UpdateAssetLifeCycleStatusAsync<TRequest>(string endpoint, Guid assetLifecycleId, TRequest data);

        /// <summary>
        ///     Connects to the Asset-microservice, notifying it about new and updated service-statuses for service-type --> SUR.
        /// </summary>
        /// <param name="assetLifecycleId"> The ID for the asset-lifecycle. </param>
        /// <param name="newServiceStatus"> The new service-status. </param>
        /// <param name="newImeis"> If the device has been replaced, a list of the new IMEI numbers. </param>
        /// <param name="newSerialNumber"> If the device has been replaced, the new serial-number. </param>
        /// <returns> The awaitable task. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="newServiceStatus"/> has a <see langword="null"/> equivalent. </exception>
        /// <exception cref="NotImplementedException"> Thrown when a not-supported, or not implemented (out of scope) service-status is triggered. </exception>
        /// <exception cref="HttpRequestException"> Thrown when we fail to update the asset microservice. </exception>
        Task UpdateAssetLifeCycleStatusForSURAsync(Guid assetLifecycleId, ServiceStatusEnum newServiceStatus, ISet<string>? newImeis, string? newSerialNumber);

        /// <summary>
        ///     Connects to the Asset-microservice, notifying it about new and updated service-statuses for service-type --> Remarketing.
        /// </summary>
        /// <param name="assetLifecycleId"> The ID for the asset-lifecycle. </param>
        /// <param name="newServiceStatus"> The new service-status. </param>
        /// <param name="newImeis"> If the device has been replaced, a list of the new IMEI numbers. </param>
        /// <param name="newSerialNumber"> If the device has been replaced, the new serial-number. </param>
        /// <returns> The awaitable task. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="newServiceStatus"/> has a <see langword="null"/> equivalent. </exception>
        /// <exception cref="NotImplementedException"> Thrown when a not-supported, or not implemented (out of scope) service-status is triggered. </exception>
        /// <exception cref="HttpRequestException"> Thrown when we fail to update the asset microservice. </exception>
        Task UpdateAssetLifeCycleStatusForRemarketingAsync(Guid assetLifecycleId, ServiceStatusEnum newServiceStatus, ISet<string>? newImeis, string? newSerialNumber);
    }
}
