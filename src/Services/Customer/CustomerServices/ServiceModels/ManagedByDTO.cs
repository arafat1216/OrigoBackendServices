using System;

namespace CustomerServices.ServiceModels
{
    public record ManagedByDTO
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }

    }
}
