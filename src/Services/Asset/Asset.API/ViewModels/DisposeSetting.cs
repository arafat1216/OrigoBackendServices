using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asset.API.ViewModels
{
    public class DisposeSetting
    {
        public Guid Id { get; init; }
        public string PayrollContactEmail { get; init; }
        public DateTime CreatedDate { get; set; }
        public IList<ReturnLocation> ReturnLocations { get; init; }

    }
}
