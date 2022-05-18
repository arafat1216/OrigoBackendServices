using System;

namespace CustomerServices.ServiceModels
{
    public record ManagerOfDTO
    {
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
}
