using AssetServices.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Globalization;

namespace AssetServices
{
    public static class MinBuyoutConfiguration
    {
        public static IList<MinBuyoutPriceBaseline> BaselineAmounts { get; private set; }
        public static void Initialize(IConfiguration configuration)
        {
            BaselineAmounts = new List<MinBuyoutPriceBaseline>();
            var sections = configuration.GetSection("MinBuyoutPrice").Get<IList<IConfigurationSection>>();
            var culture = new CultureInfo("en-US");
            foreach (var section in sections)
            {
                BaselineAmounts.Add(new MinBuyoutPriceBaseline()
                {
                    Country = section.GetSection("Country").Value,
                    AssetCategoryId = int.Parse(section.GetSection("AssetCategoryId").Value, culture),
                    Amount = decimal.Parse(section.GetSection("Amount").Value, culture)
                });
            }
        }

    }
}
