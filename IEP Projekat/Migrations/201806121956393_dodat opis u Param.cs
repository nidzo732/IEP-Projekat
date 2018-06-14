namespace IEP_Projekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dodatopisuParam : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Params", "Comment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Params", "Comment");
        }
    }
}
