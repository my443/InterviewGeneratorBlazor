using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewGeneratorBlazor.Data
{
    public class AppDbContextFactory
    {
        private readonly string _connectionString;

        public AppDbContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public AppDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite(_connectionString);

            var context = new AppDbContext(optionsBuilder.Options);

            // Ensure the database and schema exist
            context.Database.EnsureCreated();

            return context;
        }
    }
}
