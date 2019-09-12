using System.Data.Entity;

namespace DynamicOData.Models
{
    public interface ITestingModel
    {
        DbSet<CartItem> CartItems { get; set; }
        DbSet<Cart> Carts { get; set; }
        DbSet<Customer> Customers { get; set; }
        DbSet<ProductPricing> ProductPrices { get; set; }
        DbSet<Product> Products { get; set; }
    }
}