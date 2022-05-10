using System;
using System.ComponentModel.DataAnnotations;

namespace OrigoApiGateway.Models.Asset
{
    public class ReAssignmentPersonal
    {
        [Required]
        public Guid DepartmentId { get; set; }
        [Required]
        public Guid UserId { get; set; }
    }
}
