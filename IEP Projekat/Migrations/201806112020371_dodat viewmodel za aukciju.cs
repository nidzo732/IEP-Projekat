namespace IEP_Projekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dodatviewmodelzaaukciju : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auctions", "User_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Auctions", "Name", c => c.String(nullable: false, maxLength: 50));
            CreateIndex("dbo.Auctions", "User_Id");
            AddForeignKey("dbo.Auctions", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Auctions", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Auctions", new[] { "User_Id" });
            AlterColumn("dbo.Auctions", "Name", c => c.String(nullable: false));
            DropColumn("dbo.Auctions", "User_Id");
        }
    }
}
