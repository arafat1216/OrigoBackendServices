using AssetServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asset.API.ViewModels
{
    public class Label
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public LabelColor Color { get; set; }

        public Label(AssetServices.Models.CustomerLabel label)
        {
            Id = label.ExternalId;
            Text = label.Label.Text;
            Color = label.Label.Color;
        }
    }
}
