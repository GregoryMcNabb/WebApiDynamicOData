namespace DynamicOData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class custrelcartfix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Carts", "ID", "dbo.Customers");
            AddForeignKey("dbo.Carts", "CustomerID", "dbo.Customers", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Carts", "CustomerID", "dbo.Customers");
            AddForeignKey("dbo.Carts", "ID", "dbo.Customers", "ID");
        }
    }
}
