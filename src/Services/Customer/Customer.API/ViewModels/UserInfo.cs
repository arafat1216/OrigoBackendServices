using System;

namespace Customer.API.ViewModels
{
    public class UserInfo
    {
        public string UserName { get; set; }
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid DepartmentId { get; set; } = Guid.Empty;

    }
}
