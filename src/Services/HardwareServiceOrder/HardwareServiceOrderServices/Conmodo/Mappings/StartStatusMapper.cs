using HardwareServiceOrderServices.Conmodo.ApiModels;
using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrderServices.Conmodo.Mappings
{
    internal class StartStatusMapper
    {
        /// <summary>
        ///     Retrieves the Conmodo start-status that should be used with the provided <paramref name="serviceTypeId"/>.
        /// </summary>
        /// <param name="serviceTypeId"> The service-type we want to retrieve a corresponding start-status for. </param>
        /// <returns> If supported, the <see cref="StartStatus"/> that should be used with the provided <paramref name="serviceTypeId"/>.
        ///     If the provided <paramref name="serviceTypeId"/> don't have a corresponding start-status, then a exception is thrown. </returns>
        /// <exception cref="NotSupportedException"> Thrown when a unsupported <paramref name="serviceTypeId"/> is provided. </exception>
        public StartStatus FromServiceType(int serviceTypeId)
        {
            ServiceTypeEnum serviceType = (ServiceTypeEnum)serviceTypeId;

            switch (serviceType)
            {
                case ServiceTypeEnum.SUR:
                    return new StartStatus(25007, "Garanti?");

                case ServiceTypeEnum.PreSwap:
                    return new StartStatus(25456, "Preswap?");

                case ServiceTypeEnum.Remarketing:
                    return new StartStatus(25456, "Recycle");

                case ServiceTypeEnum.Null:
                case ServiceTypeEnum.Recycle:
                default:
                    throw new NotSupportedException("This provider don't support this service-type.");
            }
        }
    }
}
