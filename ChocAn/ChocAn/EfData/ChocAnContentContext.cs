using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using ChocAn.Models;

// this class will make any specific SQL table properties you might want to force set.
// entity framework will see the foreign keys and modeling properties from the ChocAn.Models
// and set up the SQL relationships accordingly. Very rarely does EF fail to create this 
// relationships correctly but if it does, this is the class where you would set the relationships in stone

namespace ChocAn.EfData
{
    public class ChocAnDb : DbContext
    {
        // set up the tables using our object class models
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<TreatmentRecord> TreatmentRecords { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // set the table name properties. i just do this to stop the double pluralizing. just a pet peeve really
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // so SQL has had several different types of 'date-time' fields to pick from. .net and SQl don't seem to line up
            // so smoothly with dates so for every date property in the class, i use these lines to force a specific datetime type
            // in the SQL tables to match. 
            modelBuilder.Entity<TreatmentRecord>().Property(p => p.EntryDate).HasColumnType("datetime2");
            modelBuilder.Entity<TreatmentRecord>().Property(p => p.TreatmentDate).HasColumnType("datetime2");
        }
    }
}
