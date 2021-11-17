using System.ComponentModel.DataAnnotations;

namespace PersonalStocks.Data
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public double StartingValue { get; set; }
        public double CurrentValue { get; set; }

        public Stock(string name) { Name = name; }

    }
}
