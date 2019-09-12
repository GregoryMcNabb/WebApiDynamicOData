namespace DynamicOData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CartItems",
                c => new
                    {
                        CartID = c.Int(nullable: false),
                        ProductID = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CartID, t.ProductID })
                .ForeignKey("dbo.Carts", t => t.CartID, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.CartID)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.Carts",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        CustomerID = c.Int(nullable: false),
                        CheckedOut = c.Boolean(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Customers", t => t.ID)
                .Index(t => t.ID);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255),
                        Email = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255),
                        Description = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ProductPricings",
                c => new
                    {
                        ProductID = c.Int(nullable: false),
                        EffectiveDate = c.DateTime(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => new { t.ProductID, t.EffectiveDate })
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.ProductID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductPricings", "ProductID", "dbo.Products");
            DropForeignKey("dbo.CartItems", "ProductID", "dbo.Products");
            DropForeignKey("dbo.CartItems", "CartID", "dbo.Carts");
            DropForeignKey("dbo.Carts", "ID", "dbo.Customers");
            DropIndex("dbo.ProductPricings", new[] { "ProductID" });
            DropIndex("dbo.Carts", new[] { "ID" });
            DropIndex("dbo.CartItems", new[] { "ProductID" });
            DropIndex("dbo.CartItems", new[] { "CartID" });
            DropTable("dbo.ProductPricings");
            DropTable("dbo.Products");
            DropTable("dbo.Customers");
            DropTable("dbo.Carts");
            DropTable("dbo.CartItems");
        }
    }
}
