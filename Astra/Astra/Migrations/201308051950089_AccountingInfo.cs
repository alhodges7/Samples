namespace Astra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccountingInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Resources", "PurchaseCost", c => c.Single(nullable: false));
            AddColumn("dbo.Resources", "PurchaseDate", c => c.DateTime());
            AddColumn("dbo.Resources", "ReplacementCost", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Resources", "ReplacementCost");
            DropColumn("dbo.Resources", "PurchaseDate");
            DropColumn("dbo.Resources", "PurchaseCost");
        }
    }
}
