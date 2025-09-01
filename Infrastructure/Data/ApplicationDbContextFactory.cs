using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Use your connection string here
            optionsBuilder.UseSqlServer("Server=RTL\\RTL;Database=StudentInfoDB;User Id=sa;Password=rtl;MultipleActiveResultSets=true;TrustServerCertificate=True");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
