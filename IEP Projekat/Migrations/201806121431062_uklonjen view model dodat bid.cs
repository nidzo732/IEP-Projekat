namespace IEP_Projekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uklonjenviewmodeldodatbid : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bids",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Increment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TimeStamp = c.DateTime(nullable: false),
                        Auction_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Auctions", t => t.Auction_Id, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.Id, cascadeDelete:false)
                .Index(t => t.Id)
                .Index(t => t.Auction_Id);
            
            DropTable("dbo.SearchViewModels");
        }
        
        public override void Down()
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
            
            DropForeignKey("dbo.Bids", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Bids", "Auction_Id", "dbo.Auctions");
            DropIndex("dbo.Bids", new[] { "Auction_Id" });
            DropIndex("dbo.Bids", new[] { "Id" });
            DropTable("dbo.Bids");
        }
    }
}
