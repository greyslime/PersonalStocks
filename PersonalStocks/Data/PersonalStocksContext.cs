using Microsoft.EntityFrameworkCore;

namespace PersonalStocks.Data
{
    public class PersonalStocksContext : DbContext
    {
        public PersonalStocksContext(DbContextOptions  options) : base(options) { }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Movement> Movements { get; set; }
    }
}
