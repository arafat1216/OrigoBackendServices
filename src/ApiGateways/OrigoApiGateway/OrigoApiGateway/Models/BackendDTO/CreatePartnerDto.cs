using System;

namespace OrigoApiGateway.Models.BackendDTO
{
    public record CreatePartnerDto
    {
        public Guid OrganizationId { get; set; }
        public Guid CallerId { get; set; }
    }
}
