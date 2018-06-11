namespace IEP_Projekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dodatTokenOrder : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TokenOrders",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Status = c.String(nullable: false),
                        TokenCount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TokenOrders");
        }
    }
}
