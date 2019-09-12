using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DynamicOData.Models
{
    public class CartItem
    {
        [Key, Column(Order =0)]
        public int CartID { get; set; }

        [Key, Column(Order = 1)]
        public int ProductID { get; set; }

        public int Quantity { get; set; }

        [ForeignKey(nameof(CartID))]
        public Cart Cart { get; set; }

        [ForeignKey(nameof(ProductID))]
        public Product Product { get; set; }
    }
}