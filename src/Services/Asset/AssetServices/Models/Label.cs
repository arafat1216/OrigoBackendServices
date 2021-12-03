using Common.Enums;
using Common.Seedwork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.Models
{
    [Owned]
    public class Label : ValueObject
    {
        // Set to protected as DDD best practice
        protected Label()
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
