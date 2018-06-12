namespace IEP_Projekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dodatcurrentammount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auctions", "CurrentAmmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Auctions", "CurrentAmmount");
        }
    }
}
