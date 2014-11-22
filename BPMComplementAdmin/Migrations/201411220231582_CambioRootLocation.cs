namespace Ultimus.AuditManager.Admin.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CambioRootLocation : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.CatProcesses", "RootLocation");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CatProcesses", "RootLocation", c => c.String(nullable: false));
        }
    }
}
