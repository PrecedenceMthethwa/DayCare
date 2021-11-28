namespace DayCare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Day : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Children", name: "Parent_Parent_Id", newName: "Parent_Id");
            RenameIndex(table: "dbo.Children", name: "IX_Parent_Parent_Id", newName: "IX_Parent_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Children", name: "IX_Parent_Id", newName: "IX_Parent_Parent_Id");
            RenameColumn(table: "dbo.Children", name: "Parent_Id", newName: "Parent_Parent_Id");
        }
    }
}
