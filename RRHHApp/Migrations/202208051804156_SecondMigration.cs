namespace RRHHApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SecondMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Licencia", "Genero", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Licencia", "Genero");
        }
    }
}
