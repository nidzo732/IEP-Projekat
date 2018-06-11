namespace IEP_Projekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tokenorderdodatuser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TokenOrders", "User_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.TokenOrders", "User_Id");
            AddForeignKey("dbo.TokenOrders", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TokenOrders", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.TokenOrders", new[] { "User_Id" });
            DropColumn("dbo.TokenOrders", "User_Id");
        }
    }
}
