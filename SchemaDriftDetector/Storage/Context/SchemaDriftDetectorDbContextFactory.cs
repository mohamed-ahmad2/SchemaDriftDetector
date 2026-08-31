using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SchemaDriftDetector.Storage.Context
{
    public class SchemaDriftDetectorDbContextFactory
        : IDesignTimeDbContextFactory<SchemaDriftDetectorDbContext>
    {
        public SchemaDriftDetectorDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SchemaDriftDetectorDbContext>();

            optionsBuilder.UseSqlite("Data Source=design_time_placeholder.db");

            return new SchemaDriftDetectorDbContext(optionsBuilder.Options);
        }
    }
}