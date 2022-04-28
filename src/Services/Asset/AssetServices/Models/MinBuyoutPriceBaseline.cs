using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.Models
{
    public class MinBuyoutPriceBaseline
    {
        public string Country { get; set; }
        public int AssetCategoryId { get; set; }
        public decimal Amount { get; set; }
    }
}
