namespace cspWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RgNameinOfferingModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Offerings", "RgName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Offerings", "RgName");
        }
    }
}
