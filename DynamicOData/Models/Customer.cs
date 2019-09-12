using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DynamicOData.Models
{
    public class Customer
    {
        public Customer()
        {
            Carts = new HashSet<Cart>();
        }

        [Key]
        public int ID { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        public virtual ICollection<Cart> Carts { get; set; }
    }
}