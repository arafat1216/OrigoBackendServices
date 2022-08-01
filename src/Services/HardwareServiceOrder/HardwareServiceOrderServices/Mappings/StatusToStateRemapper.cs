using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrderServices.Mappings
{
    /// <summary>
    ///     Converts a service-status to it's corresponding service-state.
    /// </summary>
    public static class StatusToStateRemapper
    {
        /// <summary>
        ///     Retrieves the <see cref="ServiceStateEnum">service-state value</see> that corresponds to the provided value.
        /// </summary>
        /// <param name="serviceStatus"> The value that is being converted. </param>
        /// <returns> The converted value. If the mapping was unsuccessful, it returns <see cref="ServiceStateEnum.Null"/>. </returns>
        public static ServiceStateEnum GetEnumValue(ServiceStatusEnum serviceStatus)
        {
            switch (serviceStatus)
            {
                case ServiceStatusEnum.Unknown:
                    return ServiceStateEnum.Unknown;

                case ServiceStatusEnum.Canceled:
                    return ServiceStateEnum.Canceled;

                case ServiceStatusEnum.Registered:
                case ServiceStatusEnum.RegisteredInTransit:
                case ServiceStatusEnum.RegisteredUserActionNeeded:
                    return ServiceStateEnum.Registered;

                case ServiceStatusEnum.Ongoing:
                case ServiceStatusEnum.OngoingUserActionNeeded:
                case ServiceStatusEnum.OngoingInTransit:
                case ServiceStatusEnum.OngoingReadyForPickup:
                    return ServiceStateEnum.Ongoing;

                case ServiceStatusEnum.CompletedNotRepaired:
                case ServiceStatusEnum.CompletedRepaired:
                case ServiceStatusEnum.CompletedRepairedOnWarranty:
                case ServiceStatusEnum.CompletedReplaced:
                case ServiceStatusEnum.CompletedReplacedOnWarranty:
                case ServiceStatusEnum.CompletedCredited:
                case ServiceStatusEnum.CompletedDiscarded:
                    return ServiceStateEnum.Completed;

                default:
                    return ServiceStateEnum.Null;
            }
        }

        /// <inheritdoc cref="GetEnumValue(ServiceStatusEnum)"/>
        public static ServiceStateEnum GetEnumValue(int serviceStatus)
        {
            ServiceStatusEnum serviceStatusEnum = (ServiceStatusEnum)serviceStatus;
            return GetEnumValue(serviceStatusEnum);
        }


        /// <returns> The converted value. If the mapping was unsuccessful, it returns 0. </returns>
        /// <inheritdoc cref="GetEnumValue(ServiceStatusEnum)"/>
        public static int GetId(ServiceStatusEnum serviceStatus)
        {
            return (int)GetEnumValue(serviceStatus);
        }


        /// <inheritdoc cref="GetId(ServiceStatusEnum)"/>
        public static int GetId(int serviceStatus)
        {
            ServiceStatusEnum serviceStatusEnum = (ServiceStatusEnum)serviceStatus;
            return (int)GetEnumValue(serviceStatusEnum);
        }
    }
}
