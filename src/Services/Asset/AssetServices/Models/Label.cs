using Common.Enums;
using Common.Seedwork;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AssetServices.Models
{
    [Owned]
    public class Label : ValueObject
    {
        [JsonConstructor]
        public Label()
        { }
        
        public Label(string labelText, LabelColor labelColor)
        {
            Text = labelText;
            Color = labelColor;
        }

        public string Text { get; protected set; }
        public LabelColor Color { get; protected set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Text;
            yield return Color;
        }
    }
}
