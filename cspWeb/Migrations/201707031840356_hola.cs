namespace cspWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hola : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerId = c.String(nullable: false, maxLength: 128),
                        OwnerId = c.String(),
                        CustomerName = c.String(),
                    })
                .PrimaryKey(t => t.CustomerId);
            
            CreateTable(
                "dbo.Subscriptions",
                c => new
                    {
                        SubscriptionId = c.String(nullable: false, maxLength: 128),
                        CustomerId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.SubscriptionId)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .Index(t => t.CustomerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subscriptions", "CustomerId", "dbo.Customers");
            DropIndex("dbo.Subscriptions", new[] { "CustomerId" });
            DropTable("dbo.Subscriptions");
            DropTable("dbo.Customers");
        }
    }
}
