namespace RRHHApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ThirdMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Usuario", "IsCEO", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Usuario", "IsCEO");
        }
    }
}
