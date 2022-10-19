using Common.Enums;
using Common.Seedwork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    /// <summary>
    /// Represent price
    /// </summary>
    public class Money : ValueObject
    {
        public Money():this(decimal.Zero) 
        { }
        public Money(decimal amount, CurrencyCode currencyCode = Enums.CurrencyCode.NOK)
        {
            Amount = amount;
            CurrencyCode = currencyCode.ToString();
        }

        public Money(decimal amount, string? currencyCode)
        {
            Amount = amount;
            CurrencyCode = currencyCode;
        }


        /// <summary>
        /// Amount
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Currency code
        /// </summary>
        [Column(TypeName = "char(3)")]
        public string? CurrencyCode { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return $"{Amount.ToString("0.00")} {CurrencyCode}";
        }

        public static bool operator ==(Money m1, Money m2)
        {
            return m1?.Amount == m2?.Amount && m1?.CurrencyCode == m2?.CurrencyCode;
        }

        public static bool operator !=(Money m1, Money m2)
        {
            return m1?.Amount != m2?.Amount || m1?.CurrencyCode != m2?.CurrencyCode;
        }
    }
}
