using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models
{
    public class Label
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public LabelColor Color { get; set; }

        public string ColorName
        {
            get => Enum.GetName(Color);
        }
    }
}
