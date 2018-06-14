namespace IEP_Projekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class biduklonjenrequired : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Bids", "Auction_Id", "dbo.Auctions");
            DropIndex("dbo.Bids", new[] { "Auction_Id" });
            AlterColumn("dbo.Bids", "Auction_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Bids", "Auction_Id");
            AddForeignKey("dbo.Bids", "Auction_Id", "dbo.Auctions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bids", "Auction_Id", "dbo.Auctions");
            DropIndex("dbo.Bids", new[] { "Auction_Id" });
            AlterColumn("dbo.Bids", "Auction_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Bids", "Auction_Id");
            AddForeignKey("dbo.Bids", "Auction_Id", "dbo.Auctions", "Id", cascadeDelete: true);
        }
    }
}
