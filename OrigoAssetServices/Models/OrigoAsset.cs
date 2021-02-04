using System;
using System.ComponentModel.DataAnnotations;

namespace OrigoAssetServices.Models
{
    public class OrigoAsset
    {
        /// <summary>
        /// Id of the Asset
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        public Guid SubsId { get; set; }

        public Guid DeptId { get; set; }

        public string Imei { get; set; }

        public string Vendor {get; set;}

        public int Source { get; set; }

        public int Status {get; set;}

        public string Terminal {get; set;}

        public string PhoneNumber {get; set;}

        public string UserName {get; set;}

        public string Email {get; set;}

    }
}
