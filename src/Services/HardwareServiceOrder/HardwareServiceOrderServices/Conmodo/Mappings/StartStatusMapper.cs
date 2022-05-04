using HardwareServiceOrderServices.Conmodo.ApiModels;
using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrderServices.Conmodo.Mappings
{
    internal class StartStatusMapper
    {
        public StartStatus FromServiceType(int serviceTypeId)
        {
            switch (serviceTypeId)
            {
                case (int)ServiceTypeEnum.SUR:
                    return new StartStatus(25007, "Garanti?");
                case (int)ServiceTypeEnum.SWAP:
                    return new StartStatus(25456, "Preswap?");
                default:
                    throw new NotSupportedException("This provider don't support this service-type.");
            }
        }
    }
}
