namespace IEP_Projekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datumuTokenOrderu : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TokenOrders", "Timestamp", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TokenOrders", "Timestamp");
        }
    }
}
