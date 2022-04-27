using Common.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.Models
{
    public class CategoryLifeCycleSetting : ValueObject
    {
        public int Id { get; set; }

        /// <summary>
        /// The category id that the setting is for.
        /// </summary>
        public int AssetCategoryId { get; init; }

        /// <summary>
        /// Return the name of the category based on the Category Id.
        /// </summary>
        public string AssetCategoryName
        {
            get
            {
                return AssetCategoryId switch
                {
                    1 => "Mobile phone",
                    2 => "Tablet",
                    _ => "Unknown"
                };
            }
        }

        /// <summary>
        /// the min buyout price for this category and for this customer.
        /// </summary>
        public decimal MinBuyoutPrice { get; set; } = decimal.Zero;
        public DateTime LastUpdatedDate { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this;
        }
    }
}
