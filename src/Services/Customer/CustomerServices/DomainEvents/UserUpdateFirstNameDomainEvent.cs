﻿using Common.Logging;
using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.DomainEvents
{
    class UserUpdateFirstNameDomainEvent : BaseEvent
    {
        public UserUpdateFirstNameDomainEvent(User user, string oldName) : base(user.UserId)
        {
            User = user;
            OldFirstName = oldName;
        }

        public User User { get; protected set; }

        public string OldFirstName { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"User first name changed from {OldFirstName} to {User.FirstName}.";
        }
    }
}
