using Common.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;

namespace CustomerServices.Models
{
    public class UserPreference : Entity
    {
        protected UserPreference() { }

        public UserPreference(string language,Guid callerId)
        {
            Language = language;
            CreatedBy = callerId;
        }

        [StringLength(2, ErrorMessage = "Country code max length is 2")]
        public string Language { get; set; }

        internal void SetDeleteStatus(bool isDeleted)
        {
            IsDeleted = isDeleted;
        }
    }
}