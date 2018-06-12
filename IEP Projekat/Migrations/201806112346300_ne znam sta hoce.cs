namespace IEP_Projekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class neznamstahoce : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SearchViewModels",
                c => new
                    {
                        SearchQuery = c.String(nullable: false, maxLength: 128),
                        Status = c.Int(),
                        MinPrice = c.Decimal(precision: 18, scale: 2),
                        MaxPrice = c.Decimal(precision: 18, scale: 2),
                        Page = c.Int(),
                        My = c.Boolean(),
                        MyPurchases = c.Boolean(),
                    })
                .PrimaryKey(t => t.SearchQuery);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SearchViewModels");
        }
    }
}
