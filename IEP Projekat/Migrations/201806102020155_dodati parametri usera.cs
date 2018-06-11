namespace IEP_Projekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dodatiparametriusera : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Tokens", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.AspNetUsers", "FirstName", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.AspNetUsers", "LastName", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "LastName");
            DropColumn("dbo.AspNetUsers", "FirstName");
            DropColumn("dbo.AspNetUsers", "Tokens");
        }
    }
}
