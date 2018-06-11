namespace IEP_Projekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tokenorderduzinakljuca : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.TokenOrders");
            AlterColumn("dbo.TokenOrders", "Id", c => c.String(nullable: false, maxLength: 200));
            AddPrimaryKey("dbo.TokenOrders", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.TokenOrders");
            AlterColumn("dbo.TokenOrders", "Id", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.TokenOrders", "Id");
        }
    }
}
