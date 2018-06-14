namespace IEP_Projekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dodatevalute : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auctions", "Currency", c => c.String(nullable: false));
            AddColumn("dbo.Auctions", "CurrencyPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Auctions", "CurrencyPrice");
            DropColumn("dbo.Auctions", "Currency");
        }
    }
}
