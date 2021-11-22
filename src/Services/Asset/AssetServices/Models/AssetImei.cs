using Common.Seedwork;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AssetServices.Models
{
    public class AssetImei : ValueObject
    {
        protected AssetImei() { }

        public AssetImei(long imei)
        {
            Imei = imei;
        }

        public long Imei { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Imei;
        }
    }
}
