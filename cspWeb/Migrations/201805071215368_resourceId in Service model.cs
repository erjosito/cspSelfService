namespace cspWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class resourceIdinServicemodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "ResourceId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Services", "ResourceId");
        }
    }
}
