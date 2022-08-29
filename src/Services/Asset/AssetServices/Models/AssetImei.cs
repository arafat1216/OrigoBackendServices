using Common.Seedwork;
using System.Collections.Generic;
using AutoMapper.Internal.Mappers;

namespace AssetServices.Models
{
    public class AssetImei : ValueObject
    {
        public AssetImei() { }

        public AssetImei(long imei)
        {
            Imei = imei;
        }

        public int Id { get; set; }

        public long Imei { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Imei;
        }
    }
}
