﻿using System;

namespace Customer.API.ViewModels
{
    public class User
    {
        public User(CustomerServices.Models.User user)
        {
            Id = user.UserId;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            EmployeeId = user.EmployeeId;
            MobileNumber = user.MobileNumber;
            OrganizationName = user.Customer != null ? user.Customer.Name : string.Empty;
            UserPreference = user.UserPreference != null ? new UserPreference(user.UserPreference) : null;
            IsActive = user.IsActive;
        }

        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// NB! This Will be removed in a later version
        /// </summary>
        public string MobileNumber { get; set; }
        /// <summary>
        /// NB! This Will be removed in a later version
        /// </summary>
        public string EmployeeId { get; set; }
        public UserPreference UserPreference { get; set; }
        public string OrganizationName { get; set; }
        public bool IsActive { get; set; }
    }
}