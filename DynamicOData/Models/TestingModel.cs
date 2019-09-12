namespace DynamicOData.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class TestingModel : DbContext, ITestingModel
    {
        // Your context has been configured to use a 'TestingModel' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'DynamicOData.Models.TestingModel' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'TestingModel' 
        // connection string in the application configuration file.
        public TestingModel()
            : base("name=TestingModel")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductPricing> ProductPrices { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductPricing>()
                .Property(e => e.UnitPrice)
                .HasPrecision(18, 2);
        }
    }
}