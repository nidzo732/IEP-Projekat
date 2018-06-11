namespace IEP_Projekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class startAmmountpromenjenudecimal : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Auctions", "StartAmmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Auctions", "StartAmmount", c => c.Long(nullable: false));
        }
    }
}
