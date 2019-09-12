using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DynamicOData.Models
{
    public class Product
    {
        public int ID { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<ProductPricing> Prices { get; set; }
    }
}