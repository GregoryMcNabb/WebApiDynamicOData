using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DynamicOData.Models
{
    public class ProductPricing
    {
        [Key, Column(Order = 0)]
        public int ProductID { get; set; }

        [Key, Column(Order = 1)]
        public DateTime EffectiveDate { get; set; }

        public decimal UnitPrice { get; set; }
    }
}