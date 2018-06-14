namespace IEP_Projekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class promenjenFKuBid : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Bids", "Auction_Id", "dbo.Auctions");
            DropForeignKey("dbo.Bids", "Id", "dbo.AspNetUsers");
            DropIndex("dbo.Bids", new[] { "Id" });
            DropIndex("dbo.Bids", new[] { "Auction_Id" });
            AddColumn("dbo.Bids", "AuctionId", c => c.String(nullable: false, maxLength: 200));
            AddColumn("dbo.Bids", "UserId", c => c.String(nullable: false, maxLength: 200));
            DropColumn("dbo.Bids", "Auction_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Bids", "Auction_Id", c => c.String(maxLength: 128));
            DropColumn("dbo.Bids", "UserId");
            DropColumn("dbo.Bids", "AuctionId");
            CreateIndex("dbo.Bids", "Auction_Id");
            CreateIndex("dbo.Bids", "Id");
            AddForeignKey("dbo.Bids", "Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Bids", "Auction_Id", "dbo.Auctions", "Id");
        }
    }
}
