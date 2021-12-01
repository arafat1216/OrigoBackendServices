using AssetServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asset.API.ViewModels
{
    public class NewLabel
    {
        public string Text { get; set; }
        public LabelColor Color { get; set; }
    }
}
