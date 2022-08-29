using AutoMapper;
using Customer.API.ViewModels;
using CustomerServices.ServiceModels;

namespace Customer.API.Mappings
{
    /// <summary>
    /// Defines mapping between the ExceptionMessages models.
    /// </summary>
    public class ExceptionMessagesProfile : Profile
    {
        /// <summary>
        /// Public class constructor that defines the mapping between the view model and the service model. 
        /// </summary>
        public ExceptionMessagesProfile()
        {
            CreateMap<ExceptionMessagesDTO, ExceptionMessages>();
        }
    }
}
