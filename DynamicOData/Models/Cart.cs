using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DynamicOData.Models
{
    public class Cart
    {
        public Cart()
        {
            Items = new HashSet<CartItem>();
        }

        public int ID { get; set; }

        public int CustomerID { get; set; }

        public bool CheckedOut { get; set; }

        [ForeignKey(nameof(CustomerID))]
        public virtual Customer Customer { get; set; }


        public virtual ICollection<CartItem> Items { get; set; }
    }
}