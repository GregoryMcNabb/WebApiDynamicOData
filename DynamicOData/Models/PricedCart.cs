using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DynamicOData.Models
{
    public class PricedCart : Cart
    {
        [NotMapped]
        public virtual decimal TotalPrice
        {
            get
            {
                return Math.Round(Items.Sum(i =>
                i.Quantity * i.Product.Prices
                            .OrderByDescending(p => p.EffectiveDate)
                            .FirstOrDefault()?.UnitPrice ?? 0),
                2);
            }
        }
    }
}