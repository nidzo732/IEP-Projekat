namespace IEP_Projekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dodateaukcije : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Auctions",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        Duration = c.Long(nullable: false),
                        StartAmmount = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        PictureContent = c.Binary(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Auctions");
        }
    }
}
