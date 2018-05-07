namespace cspWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DatabaseGeneratedEFupgrade : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Services");
            AlterColumn("dbo.Services", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Services", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Services");
            AlterColumn("dbo.Services", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Services", "Id");
        }
    }
}
