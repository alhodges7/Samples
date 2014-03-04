using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using Astra.Models;

namespace Astra.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<Astra.DatabaseContext.AstraContext>
    {
        public Configuration()
        {
          AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Astra.DatabaseContext.AstraContext context)
        {
          List<Resource> r = context.Resources.ToList<Resource>();

          if (r.Count > 0)
            return;

          Astra.DatabaseContext.AstraContext.AstraContextDbInitializer.SeedAll(context);
        }
    }
}
