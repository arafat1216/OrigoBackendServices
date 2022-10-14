using AutoMapper;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices
{
    public class OperatorService : IOperatorService
    {
        private readonly IOperatorRepository _operatorRepository;
        private readonly IMapper _mapper;

        public OperatorService(IOperatorRepository operatorRepository, IMapper mapper)
        {
            _operatorRepository = operatorRepository;
            _mapper = mapper;
        }

        public async Task<IList<OperatorDTO>> GetAllOperatorsAsync()
        {

            var allOperators = await _operatorRepository.GetAllOperatorsAsync(true);
            return _mapper.Map<List<OperatorDTO>>(allOperators);
        }

        public async Task<OperatorDTO?> GetOperatorAsync(int id)
        {
            var @operator =  await _operatorRepository.GetOperatorAsync(id);
            return _mapper.Map<OperatorDTO>(@operator);
        }
    }
}
