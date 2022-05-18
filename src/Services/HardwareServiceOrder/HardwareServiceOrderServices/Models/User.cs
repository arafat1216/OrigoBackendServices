using Common.Seedwork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareServiceOrderServices.Models
{
    [Owned]
    public class User
    {
        public Guid ExternalId { get; init; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; }
        private User() { }

        public User(Guid externalId, string name, string email)
        {
            ExternalId = externalId;
            Name = name;
            Email = email;
        }
    }
}
