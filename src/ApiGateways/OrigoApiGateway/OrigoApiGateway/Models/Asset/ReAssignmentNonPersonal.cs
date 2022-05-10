using System;
using System.ComponentModel.DataAnnotations;

namespace OrigoApiGateway.Models.Asset
{
    public class ReAssignmentNonPersonal
    {
        [Required]
        public Guid DepartmentId { get; set; }
    }
}
