namespace IEP_Projekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dodatiparametri : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Params",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Value = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Params");
        }
    }
}
