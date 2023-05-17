using System.Collections.Generic;

using System.Data.Entity;

namespace MVCWithEFCF1.Models
{
    public class StoreDB : DbContext
    {
        //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<StoreDbContext>());
        public StoreDB() : base()
        {
            // Database.SetInitializer(new DropCreateDatabaseAlways<StoreDB>());
            Database.SetInitializer<StoreDB>(null);
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}