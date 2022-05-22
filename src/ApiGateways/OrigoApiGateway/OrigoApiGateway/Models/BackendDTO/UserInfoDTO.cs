using System;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class UserInfoDTO
    {
        public string UserName { get; set; }
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid DepartmentId { get; set; }

    }
}
