using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Hastane.Models.Entity;

namespace Hastane.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
           : base("DatabaseContext")
        {
            Database.SetInitializer<DatabaseContext>(new CreateDatabaseIfNotExists<DatabaseContext>());
        }
        public DbSet<personel> personels { get; set; }
       
        public DbSet<hasta> hastas { get; set; }

        public DbSet<randevu> randevu { get; set; }

    }
}