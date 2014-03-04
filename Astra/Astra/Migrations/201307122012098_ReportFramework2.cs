namespace Astra.Migrations
{
  using System;
  using System.Data.Entity.Migrations;
  using MTUtil.IO;
  using MTUtil.Strings;
  
    
    public partial class ReportFramework2 : DbMigration
    {
        public override void Up()
        {
          string script = FileUtils.ReadStreamToString(FileUtils.GetEmbeddedResource(typeof(ReportFramework2).Assembly, "Astra.Migrations.201307122012098_ReportFramework2_Sprocs.txt"));
          foreach (string sqlStmt in StringUtils.SplitToList(script, "\r\nGO\r\n", StringSplitOptions.RemoveEmptyEntries))
          {
            Sql(sqlStmt);
          }
        }
        
        public override void Down()
        {
        }
    }
}
