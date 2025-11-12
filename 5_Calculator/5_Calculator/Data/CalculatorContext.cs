using Microsoft.EntityFrameworkCore;

namespace _5_Calculator.Data
{
    public class CalculatorContext : DbContext
    {
        public CalculatorContext(DbContextOptions<CalculatorContext> options)
            : base(options)
        {
        }

        public DbSet<DataInputVariant> DataInputVariants { get; set; }
    }
}
