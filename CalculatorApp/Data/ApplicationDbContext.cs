using CalculatorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CalculatorApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Calculation> Calculations { get; set; }
    }
}
