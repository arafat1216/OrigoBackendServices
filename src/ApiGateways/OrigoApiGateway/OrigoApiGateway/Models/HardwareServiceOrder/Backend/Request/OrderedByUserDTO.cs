using System;

namespace OrigoApiGateway.Models.HardwareServiceOrder.Backend.Request
{
    public class OrderedByUserDTO
    {
        public string FistName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Guid Id { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string? OrganizationNumber { get; set; }
        public Guid PartnerId { get; set; }
        public string PartnerName { get; set; }
        public string PartnerOrganizationNumber { get; set; }
    }
}
